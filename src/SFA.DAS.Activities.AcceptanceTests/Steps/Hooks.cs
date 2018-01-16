using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using BoDi;
using Nest;
using SFA.DAS.Activities.AcceptanceTests.Azure;
using SFA.DAS.Activities.AcceptanceTests.Utilities;
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
            _objectContainer.RegisterInstanceAs(new Context());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IActivitiesClient>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IAzureTopicMessageBus>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IElasticClient>());
            _objectContainer.RegisterInstanceAs(_container.GetInstance<IObjectCreator>());
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            var context = _objectContainer.Resolve<Context>();
            var client = _objectContainer.Resolve<IElasticClient>();

            await Task.WhenAll(context.GetAccounts().Select(a =>
                client.DeleteByQueryAsync<Activity>(s => s
                    .Query(q => q
                        .Term(t => t
                            .Field(f => f.AccountId)
                            .Value(a.Id)
                        )
                    )
                )
            ));
        }
    }
}
