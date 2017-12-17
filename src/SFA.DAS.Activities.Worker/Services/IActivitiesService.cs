using System.Threading.Tasks;

namespace SFA.DAS.Activities.Worker.Services
{
    public interface IActivitiesService
    {
        Task AddActivity(Activity activity);
    }
}