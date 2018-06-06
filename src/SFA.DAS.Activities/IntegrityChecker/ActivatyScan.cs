using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker
{
    public class ActivitiesScan : IActivitiesScan
    {
        private readonly IActivityDiscrepancyFinder _finder;
        private readonly IActivityDiscrepancyQueue _discrepancyQueue;

        public ActivitiesScan(
            IActivityDiscrepancyFinder finder,
            IActivityDiscrepancyQueue discrepancyQueue
            )
        {
            _finder = finder;
            _discrepancyQueue = discrepancyQueue;
        }

        public Task ScanForDiscrepanciesAsync(ActivityScanParams scanParameters, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {

                var discrepancies = BuildQueue(scanParameters);

                foreach (var discrepancy in discrepancies)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    _discrepancyQueue.Push(discrepancy);
                }

                _discrepancyQueue.AddComplete();
            }, cancellationToken);
        }

        private IEnumerable<ActivityDiscrepancy> BuildQueue(ActivityScanParams scanParameters)
        {
            IEnumerable<ActivityDiscrepancy> discrepancies;

            if (scanParameters.MaxInspections.HasValue && scanParameters.MaxInspections > -1)
            {
                discrepancies = _finder.Scan(scanParameters.ScanBatchSize, scanParameters.MaxInspections.Value);
            }
            else
            {
                discrepancies = _finder.Scan(scanParameters.ScanBatchSize);
            }

            if (scanParameters.MaxDiscrepancies.HasValue && scanParameters.MaxDiscrepancies > -1)
            {
                discrepancies = discrepancies.Take(scanParameters.MaxDiscrepancies.Value);
            }

            return discrepancies;
        }
    }
}