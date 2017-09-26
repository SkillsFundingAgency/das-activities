using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Activities.API.Types.Enums;

namespace SFA.DAS.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : BaseRepository, ITaskRepository
    {
        public TaskRepository(ActivitiesConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        { }

        public async Task<IEnumerable<Activity>> GetActivitys(string ownerId)
        {
            
        }

        public async Task<Activity> GetActivity(string ownerId, ActivityType type)
        {
            
        }

        public async Task SaveActivity(Activity activity)
        {
            
        }
    }
}
