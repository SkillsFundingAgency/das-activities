using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IActivityDiscrepancyFixer
    {
        bool CanHandle(ActivityDiscrepancy discrepancy);

        void Fix(ActivityDiscrepancy discrepancy);
    }
}