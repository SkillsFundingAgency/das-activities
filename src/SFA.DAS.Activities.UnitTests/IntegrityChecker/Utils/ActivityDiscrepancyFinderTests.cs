using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;
using SFA.DAS.Activities.IntegrityChecker.Utils;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests.IntegrityChecker.Utils
{
    [TestFixture]
    public class ActivityDiscrepancyFinderTests
    {
        [Test]
        public void Constructor_Valid_ShouldSucceed()
        {
            var fixtures = new ActivityDiscrepancyFinderTestFixtures();

            var finder = fixtures.CreateActivityDiscrepancyFinder();
        }

        /// <summary>
        ///     We're testing that the pager functions are being called the expected number of times.
        ///     The pager function will be called until it returns null or an empty enumeration. This is
        ///     true even if the actual page size is less than the requested page size. 
        /// </summary>
        /// Number of pages, normal page size, last page size, expected number of function calls
        [TestCase(1, 5, 0, 1)]
        [TestCase(1, 5, 1, 2)]
        [TestCase(1, 5, 2, 2)]
        [TestCase(1, 5, 3, 2)]
        [TestCase(1, 5, 4, 2)]
        [TestCase(1, 5, 5, 2)]

        [TestCase(2, 5, 0, 2)]
        [TestCase(2, 5, 1, 3)]
        [TestCase(2, 5, 2, 3)]
        [TestCase(2, 5, 3, 3)]
        [TestCase(2, 5, 4, 3)]
        [TestCase(2, 5, 5, 3)]

        [TestCase(10, 5, 3, 11)]
        public void Scan_CosmosWithVariousPageSizes_ShouldCallCosmosPageFunctionCorrectNumberOfTimes(int numberOfPages, int pageSize, int lastPageSize, int expectedPageCalls)
        {
            var fixtures = new ActivityDiscrepancyFinderTestFixtures()
                .AddCosmosPages(numberOfPages, pageSize, lastPageSize)
                .UsingBatchSize(pageSize)
                .AssertCosmosPageCallCount(expectedPageCalls);
        }

        [TestCase("a,b,c,d,e", "a,b,c,d,e")]
        [TestCase("a,b,c,d,e", "a,b,c,d", ActivityDiscrepancyType.NotFoundInElastic)]
        [TestCase("a,b,c,d,e", "", ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic)]
        [TestCase("a,b,c,d", "a,b,c,d,e", ActivityDiscrepancyType.NotFoundInCosmos)]
        [TestCase("", "a,b,c,d,e", ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos)]
        public void Scan_CosmosAndElastic_ShouldDetectIssuesCorrectly(string cosmosPages, string elasticPages, params ActivityDiscrepancyType[] expectedDiscrepancyTypes )
        {
            var fixtures = new ActivityDiscrepancyFinderTestFixtures()
                .AddCosmosPage(cosmosPages.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries))
                .AddElasticPage(elasticPages.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries))
                .AssertResultCount(expectedDiscrepancyTypes.Length)
                .AssertResultIssues(expectedDiscrepancyTypes);
        }
    }

    public class ActivityDiscrepancyFinderTestFixtures
    {

        private readonly Lazy<ActivityDiscrepancy[]> _results;
	    private readonly GuidMapper _guidMapper;

		public ActivityDiscrepancyFinderTestFixtures()
        {
            CosmosRepoMock = new Mock<ICosmosActivityDocumentRepository>();
            CosmosPagingDataMock = new Mock<IPagingData>();

            ElasticRepoMock = new Mock<IElasticActivityDocumentRepository>();
            ElasticPagingDataMock = new Mock<IPagingData>();
	        LoggerMock = new Mock<ILog>();

			CosmosRepoMock
				.Setup(r => r.GetActivitiesAsync(It.IsAny<IPagingData>()))
                .ReturnsAsync(GetCosmosPage)
                .Callback<IPagingData>(pagingData => LogCall(CosmosPageCalls, pagingData.RequiredPageSize));

            ElasticRepoMock
                .Setup(r => r.GetActivitiesAsync(It.IsAny<IPagingData>()))
                .ReturnsAsync(GetElasticPage)
                .Callback<IPagingData>((pagingData) => LogCall(ElasticPageCalls, pagingData.RequiredPageSize));

            BatchSize = 5;

            _results = new Lazy<ActivityDiscrepancy[]>(RunScan);
			_guidMapper = new GuidMapper();
        }

        public Mock<IPagingData> CosmosPagingDataMock { get; set; }
        public IPagingData CosmosPagingData => CosmosPagingDataMock.Object;

        public Mock<ICosmosActivityDocumentRepository> CosmosRepoMock { get; set; }
        public ICosmosActivityDocumentRepository CosmosRepo => CosmosRepoMock.Object;

        public Mock<IPagingData> ElasticPagingDataMock { get; set; }
        public IPagingData ElasticPagingData => ElasticPagingDataMock.Object;

        public Mock<IElasticActivityDocumentRepository> ElasticRepoMock { get; set; }
        public IElasticActivityDocumentRepository ElasticRepo => ElasticRepoMock.Object;

	    public Mock<ILog> LoggerMock { get; set; }
	    public ILog Logger => LoggerMock.Object;

        public List<Activity[]> CosmosPages { get; } = new List<Activity[]>();
        public List<Activity[]> ElasticPages { get; } = new List<Activity[]>();

        private void LogCall(List<PageCallDetails> log, int pageSize)
        {
            var callDetails = new PageCallDetails {PageSize =  pageSize};
            log.Add(callDetails);
        }

        public readonly List<PageCallDetails> CosmosPageCalls = new List<PageCallDetails>();
        public readonly List<PageCallDetails> ElasticPageCalls = new List<PageCallDetails>();

        public ActivityDiscrepancyFinderTestFixtures UsingBatchSize(int batchSize)
        {
            BatchSize = batchSize;
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AddCosmosPage(params string[] activityIds)
        {
            AddPage(CosmosPages, _guidMapper.MapCharsToGuids(activityIds));
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AddCosmosPages(int numberOfPagesRequired, int pageSize, int lastPageSize)
        {
            for (int p = 0; p < numberOfPagesRequired; p++)
            {
                var thisPageSize = p < numberOfPagesRequired - 1 ? pageSize : lastPageSize;

                var page = new char[thisPageSize];
                for(int r = 0; r < thisPageSize; r++)
                {
                    page[r] = (char) r;
                }
                AddPage(CosmosPages, _guidMapper.MapCharsToGuids(page));
            }

            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AddElasticPage(params string[] activityIds)
        {
            AddPage(ElasticPages, _guidMapper.MapCharsToGuids(activityIds));
            return this;
        }

        public Activity[] GetCosmosPage()
        {
            return GetPage(CosmosPages);
        }

        public Activity[] GetElasticPage()
        {
            return GetPage(ElasticPages);
        }

        public ActivityDiscrepancyFinder CreateActivityDiscrepancyFinder()
        {
            return new ActivityDiscrepancyFinder(CosmosRepo, ElasticRepo, Logger);
        }

        public ActivityDiscrepancyFinderTestFixtures AssertResultCount(int expectedNumberOfDiscrepancies)
        {
            EnsureScanCompleted();
            Assert.AreEqual(expectedNumberOfDiscrepancies, _results.Value.Length);
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertResultIssues(params ActivityDiscrepancyType[] issues)
        {
            EnsureScanCompleted();
            for (int i = 0; i < issues.Length; i++)
            {
                Assert.AreEqual(issues[i], _results.Value[i].Issues);
            }
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertCosmosPageCallCount(int expectedNumberOfCallsToCosmos)
        {
            EnsureScanCompleted();
            Assert.AreEqual(expectedNumberOfCallsToCosmos, CosmosPageCalls.Count);
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertElasticPageCallCount(int expectedNumberOfCallsToElastic)
        {
            EnsureScanCompleted();
            Assert.AreEqual(expectedNumberOfCallsToElastic, ElasticPageCalls.Count);
            return this;
        }

        public int BatchSize { get; set; }

        private void AddPage(List<Activity[]> repo, Guid[] activityIds)
        {
            var activities = activityIds.Select(id => new Activity
            {
                Id = id,
                At = DateTime.Now,
                AccountId = 123,
                Created = DateTime.Now,
                Description = "Test Activity",
                Type = ActivityType.AccountCreated
            }).ToArray();

            repo.Add(activities);
        }

        private Activity[] GetPage(List<Activity[]> pages)
        {
            if (pages.Count == 0)
            {
                return new Activity[0];
            }

            var activities = pages[0];
            pages.RemoveAt(0);
            return activities;
        }

        private void EnsureScanCompleted()
        {
            Assert.IsNotNull(_results.Value);
        }

        private ActivityDiscrepancy[] RunScan()
        {
            var finder = CreateActivityDiscrepancyFinder();

            var actualResults = finder.Scan(BatchSize).ToArray();

            return actualResults;
        }
    }

	public class GuidMapper
	{
		private Guid[] _guids = new Guid[26];

		public Guid[] MapCharsToGuids(char[] chars)
		{
			EnsureInitialised();

			return chars.Select(c => _guids[c]).ToArray();
		}

		public Guid[] MapCharsToGuids(string[] chars)
		{
			EnsureInitialised();

			return chars.Where(s => s.Length == 1 && char.IsLetter(s[0])).Select(s => _guids[s[0]]).ToArray();
		}

		private void EnsureInitialised()
		{
			if (_guids == null)
			{
				return;
			}

			_guids = new Guid[26];

			for (int i = 0; i < _guids.Length; i++)
			{
				_guids[i] = Guid.NewGuid();
			}
		}
	}

    public class PageCallDetails
    {
        public int PageSize { get; set; }
    }
}
