using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Repositories
{
    public class ElasticPagingData : IPagingData
    {
        private int _inspections;
        private bool _haveFetchedFirstPage;

        public ElasticPagingData(IActivityDocumentRepository repository, int requiredPageSize, int? maximumInspections)
        {
            Repository = repository;
            RequiredPageSize = requiredPageSize;
            MaximumInspections = maximumInspections;
            _haveFetchedFirstPage = false;
        }

        public bool MoreDataAvailable { get; set; }

        public int FromIndex { get; set; }

        public int RequiredPageSize { get; set; }
        public int CurrentPage { get; set; }
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
        public bool DataExhausted => _haveFetchedFirstPage && !MoreDataAvailable;
    }
}