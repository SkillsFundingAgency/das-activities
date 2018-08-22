using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    public class TriggeredJobRepository : ITriggeredJobRepository
    {
        private static readonly ILog Log = new NLogLogger(typeof(TriggeredJobRepository));

        private static readonly Lazy<Type[]> _allPossibleTriggers = new Lazy<Type[]>(GetAllPossibleTriggers);

        private static readonly Lazy<IEnumerable<TriggeredJob>> _allTriggeredJobs = new Lazy<IEnumerable<TriggeredJob>>(GetAllTriggeredJobs);

        private static readonly Lazy<IEnumerable<TriggeredJob<QueueTriggerAttribute>>> _queueTriggeredJobs = new Lazy<IEnumerable<TriggeredJob<QueueTriggerAttribute>>>(GetTriggeredJob<QueueTriggerAttribute>);

        private static readonly Lazy<IEnumerable<TriggeredJob<TimerTriggerAttribute>>> _timerTriggeredJobs = new Lazy<IEnumerable<TriggeredJob<TimerTriggerAttribute>>>(GetTriggeredJob<TimerTriggerAttribute>);

        public IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueuedTriggeredJobs()
        {
            return _queueTriggeredJobs.Value;
        }

        public IEnumerable<TriggeredJob<TimerTriggerAttribute>> GetScheduledJobs()
        {
            return _timerTriggeredJobs.Value;
        }

        public IEnumerable<TriggeredJob> GetTriggeredJobs()
        {
            return _allTriggeredJobs.Value;
        }

        private static IEnumerable<TriggeredJob> GetAllTriggeredJobs()
        {
            var possibleTriggers = _allPossibleTriggers.Value;

            var typesToInspect = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(assembly => assembly.FullName.StartsWith("SFA.DAS.Activities", StringComparison.InvariantCultureIgnoreCase))
                            .SelectMany(assembly => assembly.GetLoadedModules().SelectMany(m => m.GetTypes()));

            var triggeredMethods = typesToInspect
                            .SelectMany(t => t.GetMethods().Where(method => MethodHasAnyTriggers(method, possibleTriggers)));

            var result = triggeredMethods.Select(
                p => new TriggeredJob
                {
                    InvokedMethod = p,
                    ContainingClass = p.DeclaringType
                }).ToArray();

            Log.Debug($"Found {result.Length}: {string.Join(",", result.Select(tj => tj.FullName))}");
            
            return result;
        }

        private static bool MethodHasAnyTriggers(MethodInfo method, Type[] triggerTypes)
        {
            return method.GetParameters().Any(p => triggerTypes.Any(triggerType => p.IsDefined(triggerType, false)));
        }

        private static Type[] GetAllPossibleTriggers()
        {
            return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(assembly => assembly.FullName.StartsWith("Microsoft.Azure.WebJobs"))
                    .SelectMany(assembly => assembly.GetTypes().Where(t => typeof(Attribute).IsAssignableFrom(t)))
                    .Where(t => t.IsDefined(typeof(BindingAttribute)))
                    .ToArray();
        }

        private static IEnumerable<TriggeredJob<TTriggerAttribute>> GetTriggeredJob<TTriggerAttribute>() where TTriggerAttribute : Attribute
        {
            return _allTriggeredJobs.Value
                            .SelectMany(method => method.InvokedMethod.GetParameters())
                            .Where(p => p.IsDefined(typeof(TTriggerAttribute)))
                            .Select(
                                p => new TriggeredJob<TTriggerAttribute>
                                {
                                    TriggerParameter = p,
                                    Trigger = (TTriggerAttribute)p.GetCustomAttribute(typeof(TTriggerAttribute)),
                                    InvokedMethod = p.Member as MethodInfo,
                                    ContainingClass = p.Member.DeclaringType
                                })
                            .ToArray();
        }
    }
}