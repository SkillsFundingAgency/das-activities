using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.Activities.Worker.Services
{
    public class ActivitiesService : IActivitiesService
    {
        private readonly IElasticClient _client;

        public ActivitiesService(IElasticClient client)
        {
            _client = client;
        }

        public async Task AddActivity(Activity activity)
        {
            await _client.IndexAsync(activity);
        }
    }
}