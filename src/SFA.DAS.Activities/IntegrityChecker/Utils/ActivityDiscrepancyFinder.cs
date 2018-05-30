using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.IntegrityChecker.Repositories;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class ActivityDiscrepancyFinder : IActivityDiscrepancyFinder
    {
        private readonly IActivityDocumentRepository _cosmosRepository;
        private readonly IActivityDocumentRepository _elasticRepository;

        public ActivityDiscrepancyFinder(
            ICosmosActivityDocumentRepository cosmosRepository, 
            IElasticActivityDocumentRepository elasticRepository)
        {
            _cosmosRepository = cosmosRepository;
            _elasticRepository = elasticRepository;
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

        private async Task<IEnumerable<Activity>> FetchNextPageOfActivities(IPagingData pagingData)
        {
            if (pagingData.MaximumInspections.HasValue && pagingData.MaximumInspections <= pagingData.Inspections)
            {
                return new Activity[] {};
            }

            var page = await pagingData.Repository.GetActivitiesAsync(pagingData);
            pagingData.Inspections = pagingData.Inspections + page.Length;
            return page;
        }
    }
}