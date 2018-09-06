using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.FixLogging;
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
        private readonly IAzureBlobRepository _blobRepo;
        private readonly IIntegrityCheckConfiguration _integrityCheckConfiguration;

        public IntegrityCheck(
            IActivitiesScan scanner,
            IActivitiesFix fixer,
            IAzureBlobRepository blobRepo,
            IIntegrityCheckConfiguration integrityCheckConfiguration)
        {
            _scanner = scanner;
            _fixer = fixer;
            _blobRepo = blobRepo;
            _integrityCheckConfiguration = integrityCheckConfiguration;
        }

        public async Task<FixActionLogger> DoAsync(CancellationToken cancellationToken, string lognameSuffix)
        {
            var logger = new FixActionLogger();
            logger.Start();
            try
            {
                var tasks = new[]
                {
                    _scanner.ScanForDiscrepanciesAsync(_integrityCheckConfiguration, logger, cancellationToken),
                    _fixer.FixDiscrepanciesAsync(logger, cancellationToken)
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                logger.TerminalException = $"{ex.GetType().Name} - {ex.Message}";
                throw;
            }
            finally
            {
                logger.Finish();
                await SaveFixLoggerResults(logger, lognameSuffix);
            }

            return logger;
        }

        private Task SaveFixLoggerResults(FixActionLogger logger, string lognameSuffix)
        {
            return _blobRepo.SerialiseObjectToLog($"IntegrityCheckResults_{lognameSuffix}", logger);
        }
    }
}
