using PerformanceTester.Types.Types;

namespace PerformanceTester.Types.Interfaces
{
    public interface IResultLogger
    {
        void LogStore(StoreTaskDetails storeDetail);
        void LogCost(StoreTaskDetails storeDetail, IOperationCost operationCost, int groupLevel);
    }
}
