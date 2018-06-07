using SFA.DAS.Activities.IntegrityChecker.Fixers;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IFixActionLogger
    {
        void Add(FixActionLoggerItem item);
    }

    
}