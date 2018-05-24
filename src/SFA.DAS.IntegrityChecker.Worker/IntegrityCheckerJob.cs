using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Activities;

namespace SFA.DAS.IntegrityChecker.Worker
{
    public class IntegrityCheckerJob
    {
        [Singleton]
        //                                    {second} {minute} {hour} {day} {month} {day-of-week}
        public void PaymentCheck([TimerTrigger("0 30 0 * * *")] TimerInfo timer, TraceWriter logger)
        {
            // Here we jolly well go!

            //ServiceLocator.Get<>();
        }
    }
}
