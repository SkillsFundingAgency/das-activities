using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.Types;
using SFA.DAS.Activities.Client.TestHost.Interfaces;

namespace SFA.DAS.Activities.Client.TestHost.Commands
{
    internal class AggregateQueryCommand : ICommand
    {
        private readonly IActivitiesClient _client;
        private readonly IConfigProvider _configProvider;
        private readonly IResultSaver _resultSaver;

        public AggregateQueryCommand(
            IActivitiesClient client, 
            IConfigProvider configProvider,
            IResultSaver resultSaver)
        {
            _client = client;
            _configProvider = configProvider;
            _resultSaver = resultSaver;
        }

        public async Task DoAsync(CancellationToken cancellationToken)
        {
            var config = _configProvider.Get<AggregateQueryConfig>();

            foreach (var accountId in NumberRange.ToInts(config.AccountIds))
            {
                var results = await _client.GetLatestActivities(accountId);

                var tubeStops = results.Aggregates.Count();
                if (tubeStops == 0 && config.IgnoreNotFound)
                {
                    continue;
                }

                var activitiesCount = results.Aggregates.Sum(a => a.Count);

                Console.WriteLine($"Account id {accountId} found {tubeStops} tube stops covering {activitiesCount} activities");

                await _resultSaver.Save(results);

            }
        }
    }
}
