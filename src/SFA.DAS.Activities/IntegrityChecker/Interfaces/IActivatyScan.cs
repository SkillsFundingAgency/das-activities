using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    /// <summary>
    ///     Scans Cosmos and ES repos and posts issues to the <see cref="IActivityDiscrepancyQueue"/>.
    /// </summary>
    public interface IActivitiesScan
    {
        Task ScanForDiscrepanciesAsync(IIntegrityCheckConfiguration scanParameters, IFixActionReaderLogger logger, CancellationToken cancellationToken);
    }
}