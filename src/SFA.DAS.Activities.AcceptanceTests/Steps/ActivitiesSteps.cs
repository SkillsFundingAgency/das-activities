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

        [When(@"(PayeSchemeAddedMessage) message get publish")]
        public void WhenAgreement_CreatedMessageGetPublish(string message)
        {
            var bus = _objectContainer.Resolve<IAzureTopicMessageBus>();
            var messageBaseType = typeof(AccountMessageBase);
            var messageType = Type.GetType($"{messageBaseType.Namespace}.{message}, {messageBaseType.Assembly.FullName}");

            bus.PublishAsync(_objectContainer.Resolve(messageType));
        }
        
        [Then(@"I should have a (PayeSchemeAdded) Activity")]
        public async Task ThenIShouldHaveAAgreementToSignActivity(string activity)
        {
            var activitiesClient = _objectContainer.Resolve<IActivitiesClient>();
            var testData = _objectContainer.Resolve<TestData>();
            var activityType = Enum.Parse(typeof(ActivityType), activity);

            var result = await Policy
                .HandleResult<ActivitiesResult>(r => r.Activities.Any())
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7) })
                .ExecuteAsync(async () => await activitiesClient.GetActivities(new ActivitiesQuery { AccountId = testData.AccountId }));

            Check.That(result.Activities.Count()).IsEqualTo(1);
            Check.That(result.Activities.Single().Type).IsEqualTo(activityType);
        }
    }
}
