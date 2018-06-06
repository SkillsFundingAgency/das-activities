using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker
{
    public class ActivitiesFix : IActivitiesFix
    {
        private readonly IActivityDiscrepancyQueue _discrepancyQueue;
        private readonly IActivityDiscrepancyFixer[] _fixers;
        private readonly IFixActionLogger _actionLogger;

        public ActivitiesFix(
            IActivityDiscrepancyQueue discrepancyQueue, 
            IActivityDiscrepancyFixer[] fixers,
            IFixActionLogger actionLogger)
        {
            _discrepancyQueue = discrepancyQueue;
            _fixers = fixers;
            _actionLogger = actionLogger;
        }

        public Task FixDiscrepanciesAsync(CancellationToken cancellationToken)
        {
            return _discrepancyQueue.StartQueueProcessingAsync(FixDiscrepancy, cancellationToken);
        }

        private async Task FixDiscrepancy(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken)
        {
            var logItem = CreateNewLogItemForDiscrepancy(discrepancy);
            _actionLogger.Add(logItem);

            foreach(var fixer in _fixers.Where(f => f.CanHandle(discrepancy)))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await LogFixerCall(fixer, discrepancy, logItem, cancellationToken);
            }
        }

        private FixActionLoggerItem CreateNewLogItemForDiscrepancy(ActivityDiscrepancy discrepancy)
        {
            return new FixActionLoggerItem
            {
                Discrepancy = discrepancy.Issues,
                Id = discrepancy.Activity.Id
            };
        }

        private Task LogFixerCall(
            IActivityDiscrepancyFixer fixer, 
            ActivityDiscrepancy discrepancy,
            FixActionLoggerItem logItem,
            CancellationToken cancellationToken)
        {
            var handlerLogItem = new FixActionHandlerLoggerItem(fixer);
            logItem.HandledBy.Add(handlerLogItem);

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                return fixer.FixAsync(discrepancy, cancellationToken);
            }
            catch (Exception e)
            {
                handlerLogItem.Error = $"{e.GetType().Name} - {e.Message}";
                throw;
            }
            finally
            {
                sw.Stop();
                handlerLogItem.FixMSecs = sw.ElapsedMilliseconds;
            }
        }
    }
}