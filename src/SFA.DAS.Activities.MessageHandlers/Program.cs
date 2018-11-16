using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.MessageHandlers.DependencyResolution;
using SFA.DAS.Activities.MessageHandlers.MessageProcessors;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.MessageHandlers
{
    public class Program
    {
        private static readonly ILog Log = new NLogLogger(typeof(Program));

        public static void Main()
        {
            try
            {
                Log.Info($"Starting {nameof(JobHostRunner)}");

                JobHostRunner
                    .Create(IoC.InitializeIoC)
                    .WithPreLaunch(typeof(Program).GetMethod(nameof(StartMessageProcessors)))
                    .RunAndBlock();

                Log.Info($"Stopping {nameof(JobHostRunner)}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"{nameof(JobHostRunner)} has faulted");
            }
        }

        // Set when message processing is running, unset when it is in a stopped state
        public static ManualResetEventSlim MessageProcessorRunning { get; } = new ManualResetEventSlim(false);

        public static ManualResetEventSlim MessageProcessorStopped { get; } = new ManualResetEventSlim(true);

        [NoAutomaticTrigger]
        public static void StartMessageProcessors(TextWriter log, CancellationToken cancellationToken)
        {
            MessageProcessorStopped.Reset();
            MessageProcessorRunning.Set();
            try
            {
                var processors = ServiceLocator.GetAll<IMessageProcessor2>().ToArray();

                Log.Info($"Found {processors.Length} message processors");

                if (processors.Length == 0)
                {
                    return;
                }

                foreach (var messageProcessor in processors)
                {
                    Log.Info($"Starting up message processor {messageProcessor.GetType().FullName}");
                    messageProcessor
                        .RunAsync(cancellationToken);
                }

                cancellationToken.WaitHandle.WaitOne();
            }
            finally
            {
                MessageProcessorRunning.Reset();
                MessageProcessorStopped.Set();
            }
        }
    }
}
