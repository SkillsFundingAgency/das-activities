using System;
using System.Reflection;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    public class TriggeredJob
    {
        public Type ContainingClass { get; set; }
        public MethodInfo InvokedMethod { get; set; }

        public string FullName => $"{ContainingClass?.Name}.{InvokedMethod?.Name}";
    }

    public class TriggeredJob<TAttribute> : TriggeredJob where TAttribute : Attribute
    {
        public ParameterInfo TriggerParameter { get; set; }
        public TAttribute Trigger { get; set; }
    }
}