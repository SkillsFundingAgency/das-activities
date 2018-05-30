using System.Collections.Concurrent;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class NotFoundInCosmosFixer : IActivityDiscrepancyFixer
    {
        private readonly IFixActionLogger _actionLogger;

        public NotFoundInCosmosFixer(IFixActionLogger actionLogger)
        {
            _actionLogger = actionLogger;
        }

        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return discrepancy.Issues.HasFlag(ActivityDiscrepancyType.NotFoundInElastic);
        }

        public void Fix(ActivityDiscrepancy discrepancy)
        {
            _actionLogger.Add(new FixActionLoggerItem
            {
                FixerType = typeof(NotFoundInCosmosFixer),
                Discrepancy = discrepancy.Issues,
                Id = discrepancy.Id.Id
            });
        }
    }
}