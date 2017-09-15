using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.Activities.Worker.DependencyResolution;
using SFA.DAS.Activities.Worker.Providers;
//using SFA.DAS.EAS.Domain.Configuration;
//using SFA.DAS.EAS.Infrastructure.DependencyResolution;
//using SFA.DAS.EAS.Infrastructure.Logging;
using StructureMap;

namespace SFA.DAS.Activities.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is running");

            try
            {
                var paymentDataProcessor = _container.GetInstance<IActivityDataProcessor>();
                paymentDataProcessor.RunAsync(_cancellationTokenSource.Token).Wait();
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

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has been started");

            _container = new Container(c =>
            {
                //c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                //c.Policies.Add(new ConfigurationPolicy<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider"));
                //c.Policies.Add(new ConfigurationPolicy<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI"));
                //c.Policies.Add(new ConfigurationPolicy<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI"));
                //c.Policies.Add(new MessagePolicy<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider"));
                //c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry<DefaultRegistry>();
            });
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.EAS.Activities.Worker is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.EAS.Activities.Worker has stopped");
        }
    }
}
