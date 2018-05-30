using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class CosmosPagingData : IPagingData
    {
        public CosmosPagingData(IActivityDocumentRepository repository, int requiredPageSize, int? maximumInspections)
        {
            Repository = repository;
            RequiredPageSize = requiredPageSize;
            MaximumInspections = maximumInspections;
        }

        public bool MoreDataAvailable => !string.IsNullOrWhiteSpace(ContinuationToken);
        public string ContinuationToken { get; set; }
        public int RequiredPageSize { get; set; }
        public int CurrentPage { get; set; }
        public int Inspections  { get; set; }
        public IActivityDocumentRepository Repository { get; }
        public int? MaximumInspections { get; }
    }
}