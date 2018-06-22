using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class NotFoundInElasticFixer : IActivityDiscrepancyFixer
    {
        private readonly IElasticActivityDocumentRepository _elasticRepo;

        public NotFoundInElasticFixer(IElasticActivityDocumentRepository elasticRepo)
        {
            _elasticRepo = elasticRepo;
        }

        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return discrepancy.Issues.HasFlag(ActivityDiscrepancyType.NotFoundInElastic);
        }

        public Task FixAsync(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken)
        {
            return _elasticRepo.UpsertActivityAsync(discrepancy.Activity);
        }
    }
}