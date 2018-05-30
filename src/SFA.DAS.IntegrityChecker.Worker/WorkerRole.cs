using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.DependencyResolver;
using SFA.DAS.IntegrityChecker.Worker.Infrastructure;
using StructureMap;

namespace SFA.DAS.IntegrityChecker.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        private IContainer _container;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("SFA.DAS.IntegrityChecker.Worker is running");

            try
            {
                _container = WebJob.InitializeIoC();
                ServiceLocator.Initialise(_container);

                var jobHostFactory = _container.GetInstance<IJobHostFactory>();
                var host = jobHostFactory.CreateJobHost();
                host.RunAndBlock();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to start up {nameof(WorkerRole)} {ex.GetType().Name} - {ex.Message}");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.IntegrityChecker.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.IntegrityChecker.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.IntegrityChecker.Worker has stopped");
        }
    }
}
