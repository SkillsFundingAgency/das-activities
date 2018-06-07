using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class ActivityDiscrepancyFinder : IActivityDiscrepancyFinder
    {
        private readonly IActivityDocumentRepository _cosmosRepository;
        private readonly IActivityDocumentRepository _elasticRepository;
        private readonly ILog _logger;

        public ActivityDiscrepancyFinder(
            ICosmosActivityDocumentRepository cosmosRepository, 
            IElasticActivityDocumentRepository elasticRepository,
            ILog logger)
        {
            _cosmosRepository = cosmosRepository;
            _elasticRepository = elasticRepository;
            _logger = logger;
        }

        public IEnumerable<ActivityDiscrepancy> Scan(ActivityDiscrepancyFinderParameters parameters)
        {
            var cosmosPagingData = new CosmosPagingData(_cosmosRepository, parameters.BatchSize, parameters.MaxInspections);
            var elasticPagingData = new ElasticPagingData(_elasticRepository, parameters.BatchSize, parameters.MaxInspections);

            return Zipper
                .Zip(
                    () => FetchNextPageFromCosmos(cosmosPagingData, parameters.ReaderLogger), 
                    () => FetchNextPageFromElastic(elasticPagingData, parameters.ReaderLogger))
                .Where(z => z.IsMissing)
                .Select(z => new ActivityDiscrepancy(z.Item, z.IsMissingInA ? ActivityDiscrepancyType.NotFoundInCosmos : ActivityDiscrepancyType.NotFoundInElastic));
        }

        private IEnumerable<Activity> FetchNextPageFromCosmos(CosmosPagingData pagingData, IFixActionReaderLogger readerLogger)
        {
            var activities = FetchNextPageOfActivities(pagingData).Result;
            readerLogger?.IncrementCosmosActivities(pagingData.CurrentPageSize);
            return activities;
        }

        private IEnumerable<Activity> FetchNextPageFromElastic(ElasticPagingData pagingData, IFixActionReaderLogger readerLogger)
        {
            var activities = FetchNextPageOfActivities(pagingData).Result;
            readerLogger?.IncrementElasticActivities(pagingData.CurrentPageSize);
            return activities;
        }

        private static int _fetchId = 0;
        private async Task<IEnumerable<Activity>> FetchNextPageOfActivities(IPagingData pagingData)
        {
            var thisFetchId = Interlocked.Increment(ref _fetchId);

            _logger.Debug($"Fetch id:{thisFetchId} request-page-size:{pagingData.RequiredPageSize} repo:{pagingData.Repository.GetType().Name}");

            var haveReachedLimit = !pagingData.MoreDataAvailable || (
                                   pagingData.MaximumInspections.HasValue &&
                                   pagingData.MaximumInspections <= pagingData.Inspections);

            if (haveReachedLimit)
            {
                _logger.Debug($"Fetch id:{thisFetchId} terminating MoreDataAvailable:{pagingData.MoreDataAvailable} TotalInspections:{pagingData.Inspections} MaximumInspections:{pagingData.MaximumInspections??-1}");
                pagingData.CurrentPageSize = 0;
                return new Activity[0];
            }

            var page = await pagingData.Repository.GetActivitiesAsync(pagingData);
            pagingData.Inspections = pagingData.Inspections + page.Length;

            _logger.Debug($"Fetch id:{thisFetchId} actual-page-size:{page.Length} inspections-so-far:{pagingData.Inspections} max-inspections:{pagingData.MaximumInspections} has-more-data:{pagingData.MoreDataAvailable}");
            return page;
        }
    }
}