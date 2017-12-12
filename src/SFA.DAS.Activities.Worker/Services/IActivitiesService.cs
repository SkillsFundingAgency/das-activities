using System.Threading.Tasks;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.Activities.Worker.Services
{
    public interface IActivitiesService
    {
        Task AddActivity(Activity activity);
    }
}