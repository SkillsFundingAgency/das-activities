using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Worker;
using SFA.DAS.IntegrityChecker.Worker;
using SFA.DAS.IntegrityChecker.Worker.Infrastructure;
using StructureMap;

namespace SFA.DAS.Activities.Host
{
    /*
     * For info on how to set up the webjob see:
     * https://docs.microsoft.com/en-us/azure/app-service/websites-dotnet-deploy-webjobs#convert
     */

    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            new Program().Run();
        }

        private IContainer _container;

        public void Run()
        {
            Trace.TraceInformation("SFA.DAS.IntegrityChecker.Worker is running");

            try
            {
                _container = ActivitiesHost.InitializeIoC();

                ServiceLocator.Initialise(_container);

                var jobHostFactory = _container.GetInstance<IJobHostFactory>();
                var host = jobHostFactory.CreateJobHost();
                host.RunAndBlock();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to run job host {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
