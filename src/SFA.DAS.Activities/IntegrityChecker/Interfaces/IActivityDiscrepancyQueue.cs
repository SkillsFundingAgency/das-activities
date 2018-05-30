using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IActivityDiscrepancyQueue
    {
        void Push(ActivityDiscrepancy discrepancy);
        void AddComplete();
        Task StartQueueProcessingAsync(Action<ActivityDiscrepancy> action, CancellationToken cancellationToken);
    }
}