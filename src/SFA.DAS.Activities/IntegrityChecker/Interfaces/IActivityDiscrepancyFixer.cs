using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Defines a service that can fix a specific type of integrity check issue.
    /// </summary>
    public interface IActivityDiscrepancyFixer
    {
        bool CanHandle(ActivityDiscrepancy discrepancy);

        Task FixAsync(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken);
    }
}