using System;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionLoggerItem
    {
        public Type FixerType { get; set; }
        public ActivityDiscrepancyType Discrepancy { get; set; }
        public string Id { get; set; }
    }
}