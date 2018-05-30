using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class ActivityDiscrepancyQueue : IActivityDiscrepancyQueue
    {
        private readonly BlockingCollection<ActivityDiscrepancy> _fixQueue = new BlockingCollection<ActivityDiscrepancy>();

        public void Push(ActivityDiscrepancy discrepancy)
        {
            _fixQueue.Add(discrepancy);
        }

        public void AddComplete()
        {
            _fixQueue.CompleteAdding();
        }

        public Task StartQueueProcessingAsync(Action<ActivityDiscrepancy> action, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested && !_fixQueue.IsCompleted)
                {
                    var discrepancy = _fixQueue.Take(cancellationToken);

                    Task.Run(() => action(discrepancy), cancellationToken);
                }
            }, cancellationToken);
        }
    }
}