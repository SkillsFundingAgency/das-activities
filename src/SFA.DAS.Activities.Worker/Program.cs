using System;
using SFA.DAS.IntegrityChecker.Worker.CreateActivities;
using SFA.DAS.NLog.Logger;
using StructureMap;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SFA.DAS.Activities.Worker
{
    public class Program : TopshelfRoleEntryPoint
    {
        private static readonly ILog Log = new NLogLogger(typeof(Program));

        private IContainer _container;

        public static int Main()
        {
            return (int)HostFactory.Run(new Program().Configure);
        }

        protected override void Configure(HostConfigurator hostConfigurator)
        {
            try
            {
                _container = ActivityWorker.InitializeIoC();

                var cmd = _container.GetInstance<CreateActivitiesCommand>();
                cmd.CreateActivities(100);

                hostConfigurator.Service(GetService, c => c.AfterStoppingService(Cleanup)).OnException(ex => Log.Fatal(ex, "Processing failed."));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Configuration failed.");
                throw;
            }
        }

        private ServiceControl GetService()
        {
            try
            {
                return _container.GetInstance<ServiceControl>();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Initialization failed.");
                throw;
            }
        }

        private void Cleanup()
        {
            try
            {
                _container.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Cleanup failed.");
            }
        }
    }
}