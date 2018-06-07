using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Dto
{
    public class CosmosPagingData : IPagingData
    {
        private int _inspections;
        private bool _haveFetchedFirstPage;

        public CosmosPagingData(IActivityDocumentRepository repository, int requiredPageSize, int? maximumInspections)
        {
            Repository = repository;
            RequiredPageSize = requiredPageSize;
            MaximumInspections = maximumInspections;
            _haveFetchedFirstPage = false;
        }

        public bool MoreDataAvailable => !_haveFetchedFirstPage || !string.IsNullOrWhiteSpace(ContinuationToken);
        public string ContinuationToken { get; set; }
        public int RequiredPageSize { get; set; }
        public int CurrentPageSize { get; set; }

        public int Inspections
        {
            get => _inspections;
            set 
            {
                _haveFetchedFirstPage = true;
                _inspections = value;
            }
        }

        public IActivityDocumentRepository Repository { get; }
        public int? MaximumInspections { get; }
    }
}