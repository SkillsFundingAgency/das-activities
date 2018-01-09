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

        [When(@"(add_paye_scheme) message get publish")]
        public void WhenAgreement_CreatedMessageGetPublish(string message)
        {
            var testData = _objectContainer.Resolve<TestData>();
            var messagePublisher = _objectContainer.Resolve<IAzureTopicMessageBus>();

            messagePublisher.PublishAsync(new PayeSchemeAddedMessage("123/3456", testData.AccountId, "Test", "ABC"));
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
            Check.That(result.Activities.Single().Type).IsEqualTo(ActivityType.PayeSchemeAdded);
        }
    }
}
