using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.MessageHandlers.DependencyResolution;
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
                var processors = ServiceLocator.GetAll<IMessageProcessor>().ToArray();

                Log.Info($"Found {processors.Length} message processors");

                if (processors.Length == 0)
                {
                    return;
                }

                // the message processors require a cancellation token _source_, not just a cancellation token.
                var cancellationTokenSource = new CancellationTokenSource();

                var hasFaulted = false;

                foreach (var messageProcessor in processors)
                {
                    Log.Info($"Starting up message processor {messageProcessor.GetType().FullName}");
                    messageProcessor
                        .RunAsync(cancellationTokenSource)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                Log.Warn(
                                    $"{messageProcessor.GetType().FullName} has faulted - cancelling all other message handlers.");
                                hasFaulted = true;
                                cancellationTokenSource.Cancel();
                            }
                        }, cancellationToken);
                }

                // block until either webjob is cancelling or one of our message processors has faulted
                WaitHandle.WaitAny(new []
                {
                    cancellationToken.WaitHandle,
                    cancellationTokenSource.Token.WaitHandle
                });

                // if web job is cancelling then instruct our message processors to cancel also
                if(cancellationToken.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }
            finally
            {
                MessageProcessorRunning.Reset();
                MessageProcessorStopped.Set();
            }
        }
    }
}
