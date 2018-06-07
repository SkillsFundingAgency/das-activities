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
        ///     The pager function will be called until it is known that there is no more data available. 
        ///     This is indicated by the <see cref="IPagingData.DataExhausted"/> property.
        ///     For Cosmos we know we're done when we don't get a continuation token.
        ///     For ES we know we're done when we don't get a partial page of results back.
        /// </summary>
        /// Number of pages, normal page size, last page size, expected number of function calls
        [TestCase(1, 5, 0)]
        [TestCase(1, 5, 1)]
        [TestCase(1, 5, 2)]
        [TestCase(1, 5, 3)]
        [TestCase(1, 5, 4)]
        [TestCase(1, 5, 5)]

        [TestCase(2, 5, 0)]
        [TestCase(2, 5, 1)]
        [TestCase(2, 5, 2)]
        [TestCase(2, 5, 3)]
        [TestCase(2, 5, 4)]
        [TestCase(2, 5, 5)]

        [TestCase(10, 5, 3)]
        public void Scan_VariousPageSizes_ShouldCallCosmosPagerFunctionCorrectNumberOfTimes(int numberOfPages, int pageSize, int lastPageSize)
        {
            var expectedNumberOfCosmosCalls = numberOfPages;

            if (lastPageSize > pageSize)
            {
                throw new ArgumentException($"last page can only be a full page or partial page - it cannot be larger than a full page");
            }

            var fixtures = new ActivityDiscrepancyFinderTestFixtures()
                .AddCosmosPages(numberOfPages, pageSize, lastPageSize)
                .RunScan(pageSize)
                .AssertCosmosPageCallCount(expectedNumberOfCosmosCalls);
        }

        [TestCase(1, 5, 0)]
        [TestCase(1, 5, 1)]
        [TestCase(1, 5, 2)]
        [TestCase(1, 5, 3)]
        [TestCase(1, 5, 4)]
        [TestCase(1, 5, 5)]

        [TestCase(2, 5, 0)]
        [TestCase(2, 5, 1)]
        [TestCase(2, 5, 2)]
        [TestCase(2, 5, 3)]
        [TestCase(2, 5, 4)]
        [TestCase(2, 5, 5)]

        [TestCase(10, 5, 3)]
        public void Scan_VariousPageSizes_ShouldCallElasticPagerFunctionCorrectNumberOfTimes(int numberOfPages, int pageSize, int lastPageSize)
        {
            var expectedNumberOfElasticCalls = numberOfPages + (lastPageSize < pageSize ? 0 : 1);

            var fixtures = new ActivityDiscrepancyFinderTestFixtures()
                .AddElasticPages(numberOfPages, pageSize, lastPageSize)
                .RunScan(pageSize)
                .AssertElasticPageCallCount(expectedNumberOfElasticCalls);
        }

        [TestCase("a,b,c,d,e", "a,b,c,d,e")]
        [TestCase("a,b,c,d,e", "a,b,c,d", ActivityDiscrepancyType.NotFoundInElastic)]
        [TestCase("a,b,c,d,e", "", ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic, ActivityDiscrepancyType.NotFoundInElastic)]
        [TestCase("a,b,c,d", "a,b,c,d,e", ActivityDiscrepancyType.NotFoundInCosmos)]
        [TestCase("", "a,b,c,d,e", ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos, ActivityDiscrepancyType.NotFoundInCosmos)]
        public void Scan_CosmosAndElastic_ShouldDetectIssuesCorrectly(string cosmosActivities, string elasticActivities, params ActivityDiscrepancyType[] expectedDiscrepancyTypes )
        {
            var fixtures = new ActivityDiscrepancyFinderTestFixtures()
                .AddCosmosPage(cosmosActivities)
                .AddElasticPage(elasticActivities)
                .RunScan(500)
                .AssertResultCount(expectedDiscrepancyTypes.Length)
                .AssertResultIssues(expectedDiscrepancyTypes);
        }
    }

    public class ActivityDiscrepancyFinderTestFixtures
    {
        private ActivityDiscrepancy[] _results;
	    private readonly GuidMapper _guidMapper;
        private readonly List<PageCallDetails> _cosmosPageCalls = new List<PageCallDetails>();
        private readonly List<PageCallDetails> _elasticPageCalls = new List<PageCallDetails>();
        private int _normalElasticPageSize;

        public ActivityDiscrepancyFinderTestFixtures()
        {
            CosmosRepoMock = new Mock<ICosmosActivityDocumentRepository>();

            ElasticRepoMock = new Mock<IElasticActivityDocumentRepository>();

	        LoggerMock = new Mock<ILog>();

			CosmosRepoMock
				.Setup(r => r.GetActivitiesAsync(It.IsAny<IPagingData>()))
                .Callback<IPagingData>(pagingData =>
			    {
			        LogCall(_cosmosPageCalls, pagingData.RequiredPageSize);
                    SetCosmosEndOfDataMarker(pagingData);
			    })
			    .ReturnsAsync(GetCosmosPage);

            ElasticRepoMock
                .Setup(r => r.GetActivitiesAsync(It.IsAny<IPagingData>()))
                .Callback<IPagingData>((pagingData) =>
                {
                    LogCall(_elasticPageCalls, pagingData.RequiredPageSize);
                    SetElasticEndOfDataMarker(pagingData);
                })
                .ReturnsAsync(GetElasticPage);

			_guidMapper = new GuidMapper();
        }

        public Mock<ICosmosActivityDocumentRepository> CosmosRepoMock { get; set; }
        public ICosmosActivityDocumentRepository CosmosRepo => CosmosRepoMock.Object;

        public Mock<IElasticActivityDocumentRepository> ElasticRepoMock { get; set; }
        public IElasticActivityDocumentRepository ElasticRepo => ElasticRepoMock.Object;

	    public Mock<ILog> LoggerMock { get; set; }
	    public ILog Logger => LoggerMock.Object;

        public List<Activity[]> CosmosPages { get; } = new List<Activity[]>();
        public List<Activity[]> ElasticPages { get; } = new List<Activity[]>();

        public ActivityDiscrepancyFinderTestFixtures AddCosmosPage(string activityIds)
        {
            return AddCosmosPage(activityIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
        }

        public ActivityDiscrepancyFinderTestFixtures AddCosmosPage(params string[] activityIds)
        {
            AddPage(CosmosPages, _guidMapper.MapCharsToGuids(activityIds));
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AddCosmosPages(int numberOfPagesRequired, int pageSize, int lastPageSize)
        {
            return AddPages(CosmosPages, numberOfPagesRequired, pageSize, lastPageSize);
        }

        public ActivityDiscrepancyFinderTestFixtures AddElasticPages(int numberOfPagesRequired, int pageSize, int lastPageSize)
        {
            _normalElasticPageSize = pageSize;
            return AddPages(ElasticPages, numberOfPagesRequired, pageSize, lastPageSize);
        }

        public ActivityDiscrepancyFinderTestFixtures AddElasticPage(string activityIds)
        {
            return AddElasticPage(activityIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
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

        public ActivityDiscrepancyFinderTestFixtures RunScan(int batchSize)
        {
            var finder = CreateActivityDiscrepancyFinder();

            var parameters = new ActivityDiscrepancyFinderParameters
            {
                BatchSize = batchSize
            };

            _results = finder.Scan(parameters).ToArray();

            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertResultCount(int expectedNumberOfDiscrepancies)
        {
            Assert.AreEqual(expectedNumberOfDiscrepancies, _results.Length);
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertResultIssues(params ActivityDiscrepancyType[] issues)
        {
            for (int i = 0; i < issues.Length; i++)
            {
                Assert.AreEqual(issues[i], _results[i].Issues);
            }
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertCosmosPageCallCount(int expectedNumberOfCallsToCosmos)
        {
            Assert.AreEqual(expectedNumberOfCallsToCosmos, _cosmosPageCalls.Count, "Cosmos pager called incorrect number of times");
            return this;
        }

        public ActivityDiscrepancyFinderTestFixtures AssertElasticPageCallCount(int expectedNumberOfCallsToElastic)
        {
            Assert.AreEqual(expectedNumberOfCallsToElastic, _elasticPageCalls.Count, "Elastic pager called incorrect number of times");
            return this;
        }

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

        private ActivityDiscrepancyFinderTestFixtures AddPages(List<Activity[]> pages, int numberOfPagesRequired, int pageSize, int lastPageSize)
        {
            if (lastPageSize > pageSize)
            {
                throw new ArgumentException($"last page can only be a full page or partial page - it cannot be larger than a full page");
            }

            pages.Clear();

            for (int p = 0; p < numberOfPagesRequired; p++)
            {
                var thisPageSize = p < numberOfPagesRequired - 1 ? pageSize : lastPageSize;

                var page = new char[thisPageSize];
                for(int r = 0; r < thisPageSize; r++)
                {
                    page[r] = (char) r;
                }
                AddPage(pages, _guidMapper.MapCharsToGuids(page));
            }

            return this;
        }

        private void SetCosmosEndOfDataMarker(IPagingData pagingData)
        {
            var cosmosPagingData = (CosmosPagingData)pagingData;

            cosmosPagingData.ContinuationToken = CosmosPages.Count > 1 ? Guid.NewGuid().ToString() : null;
            cosmosPagingData.CurrentPageSize = CosmosPages.Count > 0 ? CosmosPages[CosmosPages.Count - 1].Length : 0;
        }

        private void SetElasticEndOfDataMarker(IPagingData pagingData)
        {
            var elasticPagingData = (ElasticPagingData) pagingData;

            elasticPagingData.CurrentPageSize = ElasticPages.Count > 0 ? ElasticPages[ElasticPages.Count-1].Length : 0;
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

        private void LogCall(List<PageCallDetails> log, int pageSize)
        {
            var callDetails = new PageCallDetails {PageSize =  pageSize};
            log.Add(callDetails);
        }
    }

    /// <summary>
    ///     This is a helper class for the tests to allow the tests to specify short activity Ids such as A, B and C rather than 
    ///     actual GUIDs.
    /// </summary>
	public class GuidMapper
	{
		private Guid[] _guids;

		public Guid[] MapCharsToGuids(char[] chars)
		{
			EnsureInitialised();

			return chars.Select(c => _guids[c]).ToArray();
		}

		public Guid[] MapCharsToGuids(string[] chars)
		{
			EnsureInitialised();

		    const int arrayLowerBoundAdjustment = 'A';

			return chars
			        .Where(s => s.Length == 1 && char.IsLetter(s[0]))
			        .Select(s => _guids[char.ToUpperInvariant(s[0]) - arrayLowerBoundAdjustment])
			        .ToArray();
		}

		private void EnsureInitialised()
		{
			if (_guids != null)
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
