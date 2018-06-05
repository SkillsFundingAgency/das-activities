using System.Threading.Tasks;

namespace SFA.DAS.Activities
{
    public interface IActivitySaver
    {
        Task<Activity> SaveActivity<TMessage>(TMessage message, ActivityType activityType) where TMessage : class;
    }
}