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
        public async Task ProcessQueueMessage(CancellationToken _cancellationTokenSource, TextWriter log)
        {
            var _container = IoC.Initialize();
            var logger = _container.GetInstance<ILog>();
            while (true)
            {
                try
                {
                    var messageProcessors = _container.GetAllInstances<IMessageProcessor>();
                    var tasks = messageProcessors.Select(x => x.RunAsync(_cancellationTokenSource)).ToArray();
                    Task.WaitAll(tasks);
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex, "Unexpected Exception");
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
