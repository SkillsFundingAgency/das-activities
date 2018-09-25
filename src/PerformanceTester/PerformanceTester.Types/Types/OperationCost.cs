using System;

namespace PerformanceTester.Types.Types
{
    public class OperationCost : IOperationCost
    {
        public OperationCost(string operation, double cost, double elapsedTicks)
        {
            this.Operation = operation;
            this.Cost = cost;
            this.ElapsedTicks = elapsedTicks;
        }

        public string Operation { get; set; }

        public double Cost { get; set; }

        public double ElapsedMSecs => ElapsedTicks / TimeSpan.TicksPerMillisecond;

        public double ElapsedTicks { get; set; }
    }
}