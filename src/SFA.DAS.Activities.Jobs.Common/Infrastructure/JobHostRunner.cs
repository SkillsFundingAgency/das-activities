using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using StructureMap;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure
{
    public class JobHostRunner
    {
        public List<MethodInfo> PreLaunch { get; } = new List<MethodInfo>();
        public Func<IContainer> ContainerProvider { get; private set; }

        public static JobHostRunner Create(Func<IContainer> containerProvider)
        {
            return new JobHostRunner().WithContainer(containerProvider);
        }

        public JobHostRunner WithPreLaunch(MethodInfo methodInfo)
        {
            PreLaunch.Add(methodInfo);
            return this;
        }

        public JobHostRunner WithContainer(Func<IContainer> containerProvider)
        {
            ContainerProvider = containerProvider;
            return this;
        }

        public IContainer Container { get; private set; }

        public void RunAndBlock()
        {
            Trace.TraceInformation($"{Assembly.GetEntryAssembly().FullName} is starting");

            try
            {
                Trace.TraceInformation($"{Assembly.GetEntryAssembly().FullName} initialising IoC");
                Container = ContainerProvider();

                ServiceLocator.Initialise(Container);

                var jobHostFactory = Container.GetInstance<IJobHostFactory>();
                var host = jobHostFactory.CreateJobHost();

                DoPreLaunch(host);

                Trace.TraceInformation($"{Assembly.GetEntryAssembly().FullName} job host starting");
                host.RunAndBlock();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to run job host {Assembly.GetEntryAssembly().FullName} - {ex.GetType().Name} - {ex.Message}");
            }
        }

        private void DoPreLaunch(JobHost jobhost)
        {
            foreach (var method in PreLaunch)
            {
                Trace.TraceInformation($"Performing pre-launch {method.DeclaringType.FullName}.{method.Name}");
                jobhost.Call(method);
            }
        }
    }
}