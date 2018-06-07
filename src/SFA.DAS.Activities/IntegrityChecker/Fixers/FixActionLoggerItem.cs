using System;
using System.Collections.Generic;
using SFA.DAS.Activities.IntegrityChecker.Dto;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionLoggerItem
    {
        private readonly List<FixActionHandlerLoggerItem> _handlers = new List<FixActionHandlerLoggerItem>();

        public ActivityDiscrepancyType Discrepancy { get; set; }
        public Guid Id { get; set; }
        public bool Handled => _handlers.Count > 0;
        public bool Success { get; private set; } = true;
    
        public IReadOnlyList<FixActionHandlerLoggerItem> HandledBy => _handlers;

        public void Add(FixActionHandlerLoggerItem item)
        {
            _handlers.Add(item);
            Success = Success && item.Success;
        }
    }
}