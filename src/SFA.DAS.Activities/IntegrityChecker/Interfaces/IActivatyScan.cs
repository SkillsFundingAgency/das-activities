using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Scans Cosmos and ES repos and posts issues to the <see cref="IActivityDiscrepancyQueue"/>.
    /// </summary>
    public interface IActivitiesScan
    {
        Task ScanForDiscrepanciesAsync(ActivityScanParams scanParameters, CancellationToken cancellationToken);
    }
}