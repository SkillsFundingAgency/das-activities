using System;
using System.Collections.Concurrent;
using System.Linq;

namespace PerformanceTester.Types.Types
{
    public class GroupOperationCost : IOperationCost
    {
        public GroupOperationCost(string operation)
        {
            Operation = operation;
            StepCosts = new ConcurrentBag<IOperationCost>();
        }

        public string Operation { get; }

        public double Cost => ElapsedTicks / TimeSpan.TicksPerMillisecond;

        public double ElapsedMSecs => StepCosts.Sum(opc => opc.Cost);

        public double ElapsedTicks { get; set; }

        public ConcurrentBag<IOperationCost> StepCosts { get; }
    }
}