using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class NotFoundInCosmosFixer : IActivityDiscrepancyFixer
    {
        private readonly ICosmosActivityDocumentRepository _cosmosRepo;

        public NotFoundInCosmosFixer(ICosmosActivityDocumentRepository cosmosRepo)
        {
            _cosmosRepo = cosmosRepo;
        }

        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return discrepancy.Issues.HasFlag(ActivityDiscrepancyType.NotFoundInCosmos);
        }

        public Task FixAsync(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken)
        {
            return _cosmosRepo.UpsertActivityAsync(discrepancy.Activity);
        }
    }
}