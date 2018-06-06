using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker
{
    /// <summary>
    ///     Scans activities in Cosmos and Elastic looking for missing data and fixing the issues.
    /// </summary>
    public class IntegrityCheck 
    {
        private readonly IActivitiesScan _scanner;
        private readonly IActivitiesFix _fixer;
        private readonly IFixActionLogger _fixLogger;
        private readonly IAzureBlobRepository _blobRepo;

        public IntegrityCheck(
            IActivitiesScan scanner,
            IActivitiesFix fixer,
            IFixActionLogger fixLogger,
            IAzureBlobRepository blobRepo)
        {
            _scanner = scanner;
            _fixer = fixer;
            _fixLogger = fixLogger;
            _blobRepo = blobRepo;
        }

        public async Task DoAsync(CancellationToken cancellationToken, string lognameSuffix)
        {
            _fixLogger.Clear();

            var tasks = new[]
            {
                _scanner.ScanForDiscrepanciesAsync(new ActivityScanParams(), cancellationToken),
                _fixer.FixDiscrepanciesAsync(cancellationToken)
            };

            await Task.WhenAll(tasks);
                
            await SaveFixLoggerResults(lognameSuffix);
        }

        private Task SaveFixLoggerResults(string lognameSuffix)
        {
            return _blobRepo.SerialiseObjectToLog($"IntegrityCheckResults_{lognameSuffix}", _fixLogger.GetFixes().ToArray());
        }
    }
}
