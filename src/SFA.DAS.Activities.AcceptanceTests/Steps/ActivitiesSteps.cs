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
            var messagePublisher = _objectContainer.Resolve<IAzureTopicMessageBus>();

            //Type type = Type.GetType($"SFA.DAS.EmployerAccounts.Events.Messages.{message}, SFA.DAS.EmployerAccounts.Events");

            Type messageType = typeof(AccountMessageBase);
            Type type = Type.GetType($"{messageType.Namespace}.{message}, {messageType.Assembly.FullName}");

            messagePublisher.PublishAsync(_objectContainer.Resolve(type));
        }
        
        [Then(@"I should have a (PayeSchemeAdded) Activity")]
        public async Task ThenIShouldHaveAAgreementToSignActivity(string activityType)
        {
            var testData = _objectContainer.Resolve<TestData>();
            var activitiesClient = _objectContainer.Resolve<IActivitiesClient>();

            var result = await Policy
                .HandleResult<ActivitiesResult>(r => r.Activities.Any())
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) })
                .ExecuteAsync(async () => await activitiesClient.GetActivities(new ActivitiesQuery { AccountId = testData.AccountId }));

            Check.That(result.Activities.Count()).IsEqualTo(1);

            Type type = Type.GetType($"SFA.DAS.Activities.{activityType}, SFA.DAS.Activities");

            var activity = Activator.CreateInstance(type);

            Check.That(result.Activities.Single().Type).IsEqualTo(activity);
        }
    }
}
