using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class ActivityDiscrepancyFinder : IActivityDiscrepancyFinder
    {
        private readonly IActivityDocumentRepository _cosmosRepository;
        private readonly IActivityDocumentRepository _elasticRepository;

        public ActivityDiscrepancyFinder(
            IActivityDocumentRepository cosmosRepository, IActivityDocumentRepository elasticRepository)
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
            var cosmosPagerInfo = new ActivityPagerInfo(_cosmosRepository, batchSize, maxInspections);
            var elasticPagerInfo = new ActivityPagerInfo(_elasticRepository, batchSize, maxInspections);

            return Zipper
                .Zip(
                    () => FetchNextPageOfActivities(cosmosPagerInfo).Result, 
                    () => FetchNextPageOfActivities(elasticPagerInfo).Result)
                .Where(z => z.IsInA == false || z.IsInB == false)
                .Select(z => new ActivityDiscrepancy(z.Item,
                    z.IsInA ? ActivityDiscrepancyType.NotFoundInElastic : ActivityDiscrepancyType.NotFoundInCosmos));
        }

        private class ActivityPagerInfo
        {
            public ActivityPagerInfo(IActivityDocumentRepository repository, int requiredPageSize, int? maximumInspections)
            {
                Repository = repository;
                RequiredPageSize = requiredPageSize;
                MaximumInspections = maximumInspections;
            }

            public int CurrentPage { get; set; }
            public int RequiredPageSize { get; }
            public int Inspections { get; set; }
            public IActivityDocumentRepository Repository { get; }
            public int? MaximumInspections { get; }
        }

        private async Task<IEnumerable<Activity>> FetchNextPageOfActivities(ActivityPagerInfo pagerInfo)
        {
            if (pagerInfo.MaximumInspections.HasValue && pagerInfo.MaximumInspections <= pagerInfo.Inspections)
            {
                return new Activity[] {};
            }

            var page = await pagerInfo.Repository.GetActivitiesAsync(pagerInfo.CurrentPage++, pagerInfo.RequiredPageSize);
            pagerInfo.Inspections = pagerInfo.Inspections + page.Activities?.Length ?? 0;
            return page.Activities;
        }
    }
}