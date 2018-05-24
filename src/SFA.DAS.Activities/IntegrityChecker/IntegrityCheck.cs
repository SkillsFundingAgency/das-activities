using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker
{
    /// <summary>
    ///     Scans activities in Cosmos and Elastic looking for missing data.
    /// </summary>
    public class IntegrityCheck
    {
        private readonly IActivitiesScan _scanner;
        private readonly IActivitiesFix _fixer;

        public IntegrityCheck(
            IActivitiesScan scanner,
            IActivitiesFix fixer)
        {
            _scanner = scanner;
            _fixer = fixer;
        }

        public Task DoAsync(CancellationToken cancellationToken)
        {
            var tasks = new[]
            {
                _scanner.ScanForDiscrepanciesAsync(new ActivityScanParams(), cancellationToken),
                _fixer.FixDiscrepanciesAsync(cancellationToken)
            };

            return Task.WhenAll(tasks);
        }
    }
}
