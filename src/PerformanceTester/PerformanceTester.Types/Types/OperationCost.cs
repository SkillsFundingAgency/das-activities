namespace PerformanceTester.Types.Types
{
    public class OperationCost : IOperationCost
    {
        public OperationCost(string operation, double cost, double elapsedMSecs)
        {
            this.Operation = operation;
            this.Cost = cost;
            this.ElapsedMSecs = elapsedMSecs;
        }

        public string Operation { get; set; }

        public double Cost { get; set; }

        public double ElapsedMSecs { get; set; }
    }
}