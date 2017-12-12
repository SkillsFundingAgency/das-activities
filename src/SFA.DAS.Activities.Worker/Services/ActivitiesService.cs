using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Client.Elastic;

namespace SFA.DAS.Activities.Worker.Services
{
    public class ActivitiesService : IActivitiesService
    {
        private const string IndexName = "activities";

        private readonly IIndexAutoMapper _indexAutoMapper;
        private readonly IElasticClient _client;

        public ActivitiesService(IIndexAutoMapper indexAutoMapper, IElasticClient client)
        {
            _indexAutoMapper = indexAutoMapper;
            _client = client;
        }

        public async Task AddActivity(Activity activity)
        {
            await _indexAutoMapper.EnureIndexExists<Activity>(IndexName);
            await _client.IndexAsync(activity, i => i.Index(IndexName));
        }
    }
}