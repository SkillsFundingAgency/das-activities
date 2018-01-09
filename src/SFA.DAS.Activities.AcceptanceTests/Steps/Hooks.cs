using TechTalk.SpecFlow;
using BoDi;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.Activities.Client;
using StructureMap;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private readonly IContainer _container;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            _container = new Container(s =>
            {
                s.AddRegistry<ActivitiesAcceptanceTestRegistry>();
            });
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IActivitiesClient>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IAzureTopicMessageBus>());
            _objectContainer.RegisterInstanceAs(new TestData());
        }
    }
}
