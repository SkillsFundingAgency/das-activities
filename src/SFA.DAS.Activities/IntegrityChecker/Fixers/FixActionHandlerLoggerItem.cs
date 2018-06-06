using System;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionHandlerLoggerItem
    {
        public FixActionHandlerLoggerItem(IActivityDiscrepancyFixer fixer)
        {
            HandlerType = fixer.GetType().FullName;
            StartTime = DateTime.Now;
        }

        public string HandlerType { get; }
        public DateTime StartTime { get; }
        public long FixMSecs { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrWhiteSpace(Error);
    }
}