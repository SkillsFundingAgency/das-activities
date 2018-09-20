using System;
using System.Collections.Concurrent;
using System.Linq;

namespace PerformanceTester.Types.Types
{
    public class GroupOperationCost : IOperationCost
    {
        public GroupOperationCost(string operation)
        {
            this.Operation = operation;
            this.StepCosts = new ConcurrentBag<IOperationCost>();
        }

        public string Operation { get; }

        public double Cost
        {
            get
            {
                return this.StepCosts.Sum<IOperationCost>((Func<IOperationCost, double>)(opc => opc.Cost));
            }
        }

        public double ElapsedMSecs
        {
            get
            {
                return this.StepCosts.Sum<IOperationCost>((Func<IOperationCost, double>)(opc => opc.ElapsedMSecs));
            }
        }

        public ConcurrentBag<IOperationCost> StepCosts { get; }
    }
}