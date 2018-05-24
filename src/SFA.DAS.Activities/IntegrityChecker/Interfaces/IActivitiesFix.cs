using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Observes the <see cref="IActivityDiscrepancyQueue"/> for issues and fixes them using 
    ///     the available <see cref="IActivityDiscrepancyFixer"/> registered.
    /// </summary>
    public interface IActivitiesFix
    {
        Task FixDiscrepanciesAsync(CancellationToken cancellationToken);
    }
}