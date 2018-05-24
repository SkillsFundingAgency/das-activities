using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker
{
    public class ActivitiesFix : IActivitiesFix
    {
        private readonly IActivityDiscrepancyQueue _discrepancyQueue;
        private readonly IActivityDiscrepancyFixer[] _fixers;

        public ActivitiesFix(
            IActivityDiscrepancyQueue discrepancyQueue, 
            IActivityDiscrepancyFixer[] fixers)
        {
            _discrepancyQueue = discrepancyQueue;
            _fixers = fixers;
        }

        public Task FixDiscrepanciesAsync(CancellationToken cancellationToken)
        {
            return _discrepancyQueue.StartQueueProcessingAsync(FixDiscrepancy, cancellationToken);
        }

        private void FixDiscrepancy(ActivityDiscrepancy discrepancy)
        {
            foreach(var fixer in _fixers.Where(f => f.CanHandle(discrepancy)))
            {
                fixer.Fix(discrepancy);
            }
        }
    }
}