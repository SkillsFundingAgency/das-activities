using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;
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

        public IEnumerable<ActivityDiscrepancy> Scan(int batchSize)
        {
            return Scan(batchSize, null);
        }

        public IEnumerable<ActivityDiscrepancy> Scan(int batchSize, int? maxInspections)
        {
            var cosmosPagingData = new CosmosPagingData(_cosmosRepository, batchSize, maxInspections);
            var elasticPagingData = new ElasticPagingData(_elasticRepository, batchSize, maxInspections);

            return Zipper
                .Zip(
                    () => FetchNextPageOfActivities(cosmosPagingData).Result, 
                    () => FetchNextPageOfActivities(elasticPagingData).Result)
                .Where(z => z.IsMissing)
                .Select(z => new ActivityDiscrepancy(z.Item, z.IsMissingInA ? ActivityDiscrepancyType.NotFoundInCosmos : ActivityDiscrepancyType.NotFoundInElastic));
        }

        private static int _fetchId = 0;
        private async Task<IEnumerable<Activity>> FetchNextPageOfActivities(IPagingData pagingData)
        {
            var thisFetchId = Interlocked.Increment(ref _fetchId);

            _logger.Debug($"Fetch id:{thisFetchId} request-page-size:{pagingData.RequiredPageSize} repo:{pagingData.Repository.GetType().Name}");

            var haveReachedLimit = pagingData.MaximumInspections.HasValue &&
                                   pagingData.MaximumInspections <= pagingData.Inspections;

            var page = haveReachedLimit ? new Activity[]{} : await pagingData.Repository.GetActivitiesAsync(pagingData);
            pagingData.Inspections = pagingData.Inspections + page.Length;

            _logger.Debug($"Fetch id:{thisFetchId} actual-page-size:{page.Length} inspections-so-far:{pagingData.Inspections} max-inspections:{pagingData.MaximumInspections} has-more-data:{pagingData.MoreDataAvailable}");
            return page;
        }
    }
}