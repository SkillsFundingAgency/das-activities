using System.Collections.Concurrent;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public class FixActionLogger : IFixActionLogger
    {
        private readonly ConcurrentBag<FixActionLoggerItem> _fixes = new ConcurrentBag<FixActionLoggerItem>();

        public void Add(FixActionLoggerItem item)
        {
            _fixes.Add(item);
        }
    }
}