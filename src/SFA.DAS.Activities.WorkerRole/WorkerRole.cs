using ESFA.DAS.Support.Indexer.Worker;
using SFA.DAS.Activities.WorkerRole.DependencyResolution;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SFA.DAS.Activities.WorkerRole
{
    public class WorkerRole : TopshelfRoleEntryPoint
    {
        protected override void Configure(HostConfigurator hostConfigurator)
        {
            var container = IoC.Initialize();
            hostConfigurator.Service(settings => container.GetInstance<HostService>(), x =>
            {
                //x.BeforeStartingService(context => _log.Info("Before starting service!!"));
                //x.AfterStoppingService(context => _log.Info("After stopping service!!"));
            });
        }

        public static int Main()
        {
            return (int) HostFactory.Run(new WorkerRole().Configure);
        }
    }
}
