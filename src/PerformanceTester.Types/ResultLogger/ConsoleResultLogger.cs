using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;

namespace PerformanceTester.Types.ResultLogger
{
    public class ConsoleResultLogger : IResultLogger
    {
        public void LogStore(StoreTaskDetails storeDetail)
        {
            Console.WriteLine(string.Format("{0} for {1} took {2} seconds. {3}", new object[4]
            {
                (object) storeDetail.Command.GetType().Name,
                (object) storeDetail.Store.Name,
                (object) storeDetail.Elapsed.TotalSeconds,
                storeDetail.Success ? (object) "Successful" : (object) "Failed"
            }));
        }

        public void LogCost(StoreTaskDetails storeDetail, IOperationCost operationCost, int groupLevel)
        {
            Console.WriteLine("{0}{1,-20}  {2:F1} {3:F2} msecs", new object[4]
            {
                (object) "-> ".PadLeft(groupLevel * 2),
                (object) operationCost.Operation,
                (object) operationCost.Cost,
                (object) operationCost.ElapsedMSecs
            });
        }
    }
}
