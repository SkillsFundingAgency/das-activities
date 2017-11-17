using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Infrastructure.DependencyResolution.Configuration.Policies;
using SFA.DAS.Activities.Worker.DependencyResolution;
using StructureMap;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        private const string ConfigName = "SFA.DAS.Activities";
        private const string WorkerName = "SFA.DAS.Activities.Worker";

        public override void Run()
        {
            Trace.TraceInformation($"{WorkerName} is running");

            try
            {
                var messageProcessors = _container.GetAllInstances<IMessageProcessor>();
                var tasks = messageProcessors.Select(x => x.RunAsync(_cancellationTokenSource.Token)).ToArray();
                Task.WaitAll(tasks);
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnStart();

            Trace.TraceInformation($"{WorkerName} has been started");

            _container = new Container(c =>
            {
                //c.Policies.Add(new MessagePublisherPolicy());
                c.Policies.Add(new MessageSubscriberPolicy());
                c.AddRegistry<DefaultRegistry>();
            });
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation($"{WorkerName} is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation($"{WorkerName} has stopped");
        }
    }
}
