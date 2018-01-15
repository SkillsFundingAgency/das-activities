using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using BoDi;
using NFluent;
using Polly;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    [Binding]
    public class ActivitiesSteps
    {
        private readonly IObjectContainer _objectContainer;

        public ActivitiesSteps(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [When(@"([^ ]*) message get publish")]
        public void WhenMessageGetPublish(string message)
        {
            var bus = _objectContainer.Resolve<IAzureTopicMessageBus>();
            var messageBaseType = typeof(AccountMessageBase);
            var messageType = Type.GetType($"{messageBaseType.Namespace}.{message}, {messageBaseType.Assembly.FullName}");

            bus.PublishAsync(_objectContainer.Resolve(messageType));
        }

        [Then(@"I should have a ([^ ]*) Activity")]
        public async Task ThenIShouldHaveAnActivity(string activity)
        {
            var activitiesClient = _objectContainer.Resolve<IActivitiesClient>();
            var accountId = _objectContainer.Resolve<TestData>().AccountId;
            var activityType = Enum.Parse(typeof(ActivityType), activity);

            var result = await Policy
                .HandleResult<ActivitiesResult>(r => 
                {
                    return !r.Activities.Any();
                })
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7) })
                .ExecuteAsync(async () => 
                {
                    Console.WriteLine($"Quering {activityType} activity for Accoount id {accountId}");
                    var response = await activitiesClient.GetActivities(new ActivitiesQuery { AccountId = accountId });
                    return response;
                });

            Check.That(result.Activities.Count()).IsEqualTo(1);
            Check.That(result.Activities.Single().Type).IsEqualTo(activityType);
        }
    }
}
