using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class NotFoundInElasticFixer : IActivityDiscrepancyFixer
    {
        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return discrepancy.Issues.HasFlag(ActivityDiscrepancyType.NotFoundInElastic);
        }

        public void Fix(ActivityDiscrepancy discrepancy)
        {
        }
    }
}