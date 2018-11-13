using System;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.Types;
using SFA.DAS.Activities.Client.TestHost.Interfaces;
using SFA.DAS.Activities.Extensions;
using SFA.DAS.Activities.Localizers;

namespace SFA.DAS.Activities.Client.TestHost.Commands
{
    internal class TestLocalizerCommand : ICommand
    {
        private readonly IActivitiesClient _client;
        private readonly IConfigProvider _configProvider;
        private readonly IResultSaver _resultSaver;

        public TestLocalizerCommand(
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
            var config = _configProvider.Get<TestLocalizerConfig>();

            foreach (var accountId in NumberRange.ToInts(config.AccountIds))
            {
                Console.WriteLine($"Looking for account {accountId}");
                var results = await _client.GetActivities(new ActivitiesQuery {AccountId = accountId});

                Console.WriteLine($"Account has {results.Total} activities");
                foreach (var activity in results.Activities)
                {
                    Console.Write($"Activity {activity.Id} {activity.Type}");
                    TryAction(activity, TrySingular, "Singular");
                    TryAction(activity, TryPlural, "Plural");
                    Console.WriteLine();
                }
            }
        }

        private void TryAction(Activity activity, Action<IActivityLocalizer, Activity> action, string test)
        {
            try
            {
                Console.Write($" {test}:");
                var localizer = activity.Type.GetLocalizer();
                action(localizer, activity);
                Console.Write("okay");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed!");
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
            }
        }

        private void TrySingular(IActivityLocalizer localizer, Activity activity)
        {
            var text = localizer.GetSingularText(activity);
        }

        private void TryPlural(IActivityLocalizer localizer, Activity activity)
        {
            var text = localizer.GetPluralText(activity,2);
        }
    }
}
