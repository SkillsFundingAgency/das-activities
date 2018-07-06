using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Activities.Jobs.Common.Infrastructure;
using SFA.DAS.Activities.MessageHandlers.DependencyResolution;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.MessageHandlers
{
    public class Program
    {
        public static void Main()
        {
            JobHostRunner
                .Create(IoC.InitializeIoC)
                .WithPreLaunch(typeof(Program).GetMethod(nameof(StartMessageProcessors)))
                .RunAndBlock();
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

                log.WriteLine($"Found {processors.Length} message processors");

                if (processors.Length == 0)
                {
                    return;
                }

                // the message processors require a cancellation token _source_, not just a cancellation token.
                var cancellationTokenSource = new CancellationTokenSource();

                var hasFaulted = false;

                foreach (var messageProcessor in processors)
                {
                    log.WriteLine($"Starting up message processor {messageProcessor.GetType().FullName}");
                    messageProcessor
                        .RunAsync(cancellationTokenSource)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                log.WriteLine(
                                    $"{messageProcessor.GetType().FullName} has faulted - canceling all other message handlers.");
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
