using System;
using SFA.DAS.Activities.Worker.Policies;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SFA.DAS.Activities.Worker
{
    public class WorkerRole : TopshelfRoleEntryPoint
    {
        private IContainer _container;

        public static int Main()
        {
            return (int)HostFactory.Run(new WorkerRole().Configure);
        }

        protected override void Configure(HostConfigurator hostConfigurator)
        {
            try
            {
                _container = new Container(c =>
                {
                    c.AddRegistry<ActivitiesRegistry>();
                    c.AddRegistry<ActivitiesWorkerRegistry>();
                    c.Policies.Add(new MessageSubscriberPolicy(() => _container.GetInstance<ActivitiesServiceBusConfiguration>(), "SFA.DAS.Activities"));
                });
                
                hostConfigurator.Service(s => _container.GetInstance<HostService>());
            }
            catch (Exception ex)
            {
                new NLogLogger().Fatal(ex, "Unhandled exception.");
            }
        }
    }
}