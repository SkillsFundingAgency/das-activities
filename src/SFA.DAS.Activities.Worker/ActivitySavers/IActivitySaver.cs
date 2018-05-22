using System.Threading.Tasks;

namespace SFA.DAS.Activities.Worker.ActivitySavers
{
    public interface IActivitySaver
    {
        Task SaveActivity<TMessage>(TMessage message, ActivityType activityType) where TMessage : class;
    }
}