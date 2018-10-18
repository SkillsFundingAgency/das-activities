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

        public double Cost => StepCosts.Sum(opc => opc.Cost); 

        public double ElapsedMSecs => ElapsedTicks / TimeSpan.TicksPerMillisecond; 

        public double ElapsedTicks => StepCosts.Sum(opc => opc.ElapsedTicks);

        public ConcurrentBag<IOperationCost> StepCosts { get; }
    }
}