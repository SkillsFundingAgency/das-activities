using System;
using System.IO;
using System.Threading;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc;
using SFA.DAS.Activities.Jobs.DependencyResolution;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs
{
    public class Program
    {
        private static readonly ILog Log = new NLogLogger(typeof(Program));

        static void Main()
        {
            try
            {
                Log.Info($"Starting {nameof(JobHostRunner)}");

                JobHostRunner
                    .Create(IoC.InitializeIoC)
                    .WithPreLaunch(typeof(Program).GetMethod(nameof(EnsureAllQueuesCreated)))
                    .RunAndBlock();

                Log.Info($"Stopping {nameof(JobHostRunner)}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{nameof(JobHostRunner)} has faulted");
            }
        }

        [NoAutomaticTrigger]
        public static void EnsureAllQueuesCreated(TextWriter log, CancellationToken cancellationToken)
        {
            var webjobhelper = ServiceLocator.Get<IAzureWebJobHelper>();
            webjobhelper.EnsureAllQueuesForTriggeredJobs();
        }
    }
}
