using System;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;

namespace PerformanceTester.Types.ResultLogger
{
    public class RunDetailsVisitor
    {
        public void Visit(RunDetails details, IResultLogger[] loggers)
        {
            foreach (StoreTaskDetails storeDetail in details.StoreDetails)
            {
                Write(loggers, logger => logger.LogStore(storeDetail));

                VisitCosts(storeDetail, storeDetail.Cost, 1, loggers);
            }
        }

        private void VisitCosts(StoreTaskDetails storeDetail, IOperationCost operationCost, int groupLevel, IResultLogger[] loggers)
        {
            Write(loggers, logger => logger.LogCost(storeDetail, operationCost, groupLevel));

            var isGroupLevel = operationCost is GroupOperationCost;

            if (isGroupLevel)
            {
                foreach (IOperationCost stepCost in ((GroupOperationCost) operationCost).StepCosts)
                {
                    VisitCosts(storeDetail, stepCost, groupLevel + 1, loggers);
                }
            }
        }

        private void Write(IResultLogger[] loggers, Action<IResultLogger> logAction)
        {
            foreach (var logger in loggers)
            {
                logAction(logger);
            }
        }
    }
}