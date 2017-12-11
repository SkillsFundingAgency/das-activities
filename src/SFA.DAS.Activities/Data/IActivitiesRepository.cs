using System.Threading.Tasks;
using SFA.DAS.Activities.Models;

namespace SFA.DAS.Activities.Data
{
    public interface IActivitiesRepository
    {
        Task SaveActivity(Activity activity);
    }
}
