using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Activities.IntegrityChecker;

namespace SFA.DAS.Activities.Jobs
{
    public class IntegrityCheckerJob
    {
        [Singleton]
        public void IntegrityCheck(
            //            {second} {minute} {hour} {day} {month} {day-of-week}
            [TimerTrigger("0 30 3 20 4 1")] TimerInfo timer,
            TraceWriter logger,
            CancellationToken cancellationToken)
        {
            var integrityCheck = ServiceLocator.Get<IntegrityCheck>();

            var task = integrityCheck.DoAsync(cancellationToken, "_scheduledJob");

            task.Wait(cancellationToken);
        }
    }
}
