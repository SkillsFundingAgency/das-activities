using System.Collections.Generic;

namespace SFA.DAS.Activities.IntegrityChecker.Fixers
{
    public interface IFixActionLogger
    {
        void Add(FixActionLoggerItem item);

        IEnumerable<FixActionLoggerItem> GetFixes();

        void Clear();
    }
}