using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionLogger : IFixActionLogger
    {
        private readonly ConcurrentBag<FixActionLoggerItem> _fixes = new ConcurrentBag<FixActionLoggerItem>();

        public void Add(FixActionLoggerItem item)
        {
            _fixes.Add(item);
        }

        public IEnumerable<FixActionLoggerItem> GetFixes()
        {
            return _fixes;
        }

        public void Clear()
        {
            FixActionLoggerItem someItem;
            while (!_fixes.IsEmpty)
            {
                _fixes.TryTake(out someItem);
            }
        }
    }
}