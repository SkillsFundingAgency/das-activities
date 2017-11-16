using MediatR;
using SFA.DAS.Activities.Application.Repositories;
using StructureMap;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Activities.DataAccess.Repositories;

namespace SFA.DAS.Activities.WorkerRole.DependencyResolution
{


    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            AddMediatrRegistrations();
            RegisterLogger();
            For<IActivitiesRepository>().Use<ActivitiesRepository>();
        }


        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

        private void RegisterLogger()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null)).AlwaysUnique();
        }
    }
}