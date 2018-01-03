using System;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SFA.DAS.Activities.Worker
{
    public class WorkerRole : TopshelfRoleEntryPoint
    {
        private static readonly ILog Log = new NLogLogger(typeof(WorkerRole));
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
                    c.AddRegistry<ActivitiesWorkerRegistry>();
                });

                hostConfigurator.Service(Init).OnException(ex => Log.Fatal(ex, "Processing failed."));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Configuration failed.");
                throw;
            }
        }

        private ServiceControl Init()
        {
            try
            {
                return _container.GetInstance<HostService>();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Initialization failed.");
                throw;
            }
        }
    }
}