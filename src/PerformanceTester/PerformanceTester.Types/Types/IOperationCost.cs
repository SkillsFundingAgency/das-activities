namespace PerformanceTester.Types.Types
{
    public interface IOperationCost
    {
        string Operation { get; }
        double Cost { get; }
        double ElapsedMSecs { get; }
    }
}

