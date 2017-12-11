using StructureMap;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SFA.DAS.Activities.Worker
{
    public class WorkerRole : TopshelfRoleEntryPoint
    {
        public static int Main()
        {
            return (int)HostFactory.Run(new WorkerRole().Configure);
        }

        protected override void Configure(HostConfigurator hostConfigurator)
        {
            var container = new Container(c =>
            {
                c.AddRegistry<SFA.DAS.Activities.Registry>();
                c.AddRegistry<SFA.DAS.Activities.Worker.Registry>();
            });

            hostConfigurator.Service(settings => container.GetInstance<HostService>(), c =>
            {
                //x.BeforeStartingService(context => _log.Info("Before starting service!!"));
                //x.AfterStoppingService(context => _log.Info("After stopping service!!"));
            });
        }
    }
}