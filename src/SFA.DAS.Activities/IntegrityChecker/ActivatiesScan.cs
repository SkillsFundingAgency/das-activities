using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.Configuration;
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

        public Task ScanForDiscrepanciesAsync(IIntegrityCheckConfiguration scanParameters, IFixActionReaderLogger logger, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {

                var discrepancies = BuildQueue(scanParameters, logger);

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

        private IEnumerable<ActivityDiscrepancy> BuildQueue(IIntegrityCheckConfiguration scanParameters, IFixActionReaderLogger logger)
        {
            var parameters = new ActivityDiscrepancyFinderParameters
            {
                BatchSize = scanParameters.CosmosPageSize,
                MaxInspections = scanParameters.MaxInspections,
                ReaderLogger = logger
            };

            IEnumerable<ActivityDiscrepancy> discrepancies = _finder.Scan(parameters);

            if (scanParameters.MaxDiscrepancies.HasValue && scanParameters.MaxDiscrepancies > -1)
            {
                discrepancies = discrepancies.Take(scanParameters.MaxDiscrepancies.Value);
            }

            return discrepancies;
        }
    }
}