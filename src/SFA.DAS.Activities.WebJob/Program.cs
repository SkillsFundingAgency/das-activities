using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.RunLocal();
            }
            else
            {
                config.RunCloud();
            }
        }
    }

    public static class JobHostConfigurationExtensions
    {
        public static void RunLocal(this JobHostConfiguration config)
        {
            config.UseDevelopmentSettings();
            var cancellationTokenSource = new CancellationTokenSource();
            var task = new Functions().ProcessMethod(cancellationTokenSource.Token, Console.Out);
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Task.Delay(TimeSpan.FromMilliseconds(50));
            }
        }

        public static void RunCloud(this JobHostConfiguration config)
        {
            var host = new JobHost(config);
            host.CallAsync(typeof(Functions).GetMethod("ProcessMethod"));
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
