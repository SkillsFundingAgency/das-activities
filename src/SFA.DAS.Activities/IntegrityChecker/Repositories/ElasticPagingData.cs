using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class ElasticPagingData : IPagingData
    {
        public ElasticPagingData(IActivityDocumentRepository repository, int requiredPageSize, int? maximumInspections)
        {
            Repository = repository;
            RequiredPageSize = requiredPageSize;
            MaximumInspections = maximumInspections;
        }

        public bool MoreDataAvailable { get; set; }

        public int FromIndex { get; set; }

        public string ContinuationToken { get; set; }
        public int RequiredPageSize { get; set; }
        public int CurrentPage { get; set; }
        public int Inspections { get; set; }
        public IActivityDocumentRepository Repository { get; }
        public int? MaximumInspections { get; }
    }
}