using System;
using System.Collections.Generic;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionLoggerItem
    {
        public Type FixerType { get; set; }
        public ActivityDiscrepancyType Discrepancy { get; set; }
        public Guid Id { get; set; }
        public List<FixActionHandlerLoggerItem> HandledBy = new List<FixActionHandlerLoggerItem>();
    }
}