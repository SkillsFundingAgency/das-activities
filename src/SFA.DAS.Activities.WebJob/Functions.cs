using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.WebJob.DependencyResolution;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.WebJob
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public async Task ProcessMethod(CancellationToken _cancellationTokenSource, TextWriter log)
        {
            var container = IoC.Initialize();
            var logger = container.GetInstance<ILog>();
            logger.Info("Starting WebJob...");
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var messageProcessors = container.GetAllInstances<IMessageProcessor>();
                    var tasks = messageProcessors.Select(x => x.RunAsync(_cancellationTokenSource)).ToArray();
                    Task.WaitAll(tasks);
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, "Unexpected Exception");
                    log.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
            logger.Info("Stopping WebJob...");

        }
    }
}
