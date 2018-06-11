using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker;

namespace SFA.DAS.IntegrityChecker.Worker
{
    public class IntegrityCheckerJob
    {
        public void IntegrityCheck(
            //            {second} {minute} {hour} {day} {month} {day-of-week}
            [TimerTrigger("0 30 3 * * 1")] TimerInfo timer,
            TraceWriter logger,
            CancellationToken cancellationToken)
        {
            var integrityCheck = ServiceLocator.Get<IntegrityCheck>();

            var task = integrityCheck.DoAsync(cancellationToken, "_scheduledJob");

            task.Wait(cancellationToken);
        }
    }
}
