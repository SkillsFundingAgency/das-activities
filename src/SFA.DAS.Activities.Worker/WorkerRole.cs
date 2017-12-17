using System;
using SFA.DAS.Activities;
using SFA.DAS.NLog.Logger;
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
            try
            {
                var container = new Container(c =>
                {
                    c.AddRegistry<ActivitiesRegistry>();
                    c.AddRegistry<ActivitiesWorkerRegistry>();
                });

                hostConfigurator.Service(settings => container.GetInstance<HostService>(), c =>
                {
                    //c.BeforeStartingService(context => _log.Info("Before starting service!!"));
                    //c.AfterStoppingService(context => _log.Info("After stopping service!!"));
                });
            }
            catch (Exception ex)
            {
                new NLogLogger().Fatal(ex, "Unhandled exception.");
            }
        }
    }
}