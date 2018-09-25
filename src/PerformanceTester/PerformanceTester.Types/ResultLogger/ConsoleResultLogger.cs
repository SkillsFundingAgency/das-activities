// Decompiled with JetBrains decompiler
// Type: PerformanceTester.Types.ResultLogger.ConsoleResultLogger
// Assembly: PerformanceTester.Types, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 811E2CD7-D50B-4F41-93E3-583B7CD698DE
// Assembly location: C:\temp\scrap\PerformanceTester\PerformanceTester\bin\Debug\PerformanceTester.Types.dll

using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;

namespace PerformanceTester.Types.ResultLogger
{
    public class ConsoleResultLogger : IResultLogger
    {
        public void Log(RunDetails details)
        {
            foreach (StoreTaskDetails storeDetail in details.StoreDetails)
            {
                Console.WriteLine(string.Format("{0} for {1} took {2} seconds. {3}", new object[4]
                {
                    (object) storeDetail.Command.GetType().Name,
                    (object) storeDetail.Store.Name,
                    (object) storeDetail.Elapsed.TotalSeconds,
                    storeDetail.Success ? (object) "Successful" : (object) "Failed"
                }));
                this.LogCosts(storeDetail.Cost, 1);
            }
        }

        private void LogCosts(IOperationCost operationCost, int level)
        {
            Console.WriteLine("{0}{1,-20}  {2:F1} {3:F2} msecs", new object[4]
            {
                (object) "-> ".PadLeft(level * 2),
                (object) operationCost.Operation,
                (object) operationCost.Cost,
                (object) operationCost.ElapsedMSecs
            });

            if (!(operationCost is GroupOperationCost))
                return;

            foreach (IOperationCost stepCost in ((GroupOperationCost)operationCost).StepCosts)
                this.LogCosts(stepCost, level + 1);
        }
    }
}
