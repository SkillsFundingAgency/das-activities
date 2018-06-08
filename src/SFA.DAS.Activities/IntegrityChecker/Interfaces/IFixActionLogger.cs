using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.FixLogging;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IFixActionLogger
    {
        void Add(FixActionLoggerItem item);
    }

    
}