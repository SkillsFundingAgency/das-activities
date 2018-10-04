using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Activities.Client.TestHost.Interfaces;

namespace SFA.DAS.Activities.Client.TestHost.Commands
{
    internal class AggregateQueryCommand : ICommand
    {
        private readonly IActivitiesClient _client;
        private readonly IConfigProvider _configProvider;

        public AggregateQueryCommand(IActivitiesClient client, IConfigProvider configProvider)
        {
            _client = client;
            _configProvider = configProvider;
        }

        public Task DoAsync(CancellationToken cancellationToken)
        {
            var config = _configProvider.Get<AggregateQueryConfig>();

            return _client.GetLatestActivities(config.AccountId);
        }
    }
}
