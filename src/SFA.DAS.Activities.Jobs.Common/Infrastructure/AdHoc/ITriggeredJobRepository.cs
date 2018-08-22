using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    /// <summary>
    ///		A repository for discovering all jobs triggered by Azure SDK triggers.
    /// </summary>
    public interface ITriggeredJobRepository
    {
        /// <summary>
        ///		Returns all tasks that are triggered by a queue message (on any queue)
        /// </summary>
        IEnumerable<TriggeredJob<QueueTriggerAttribute>> GetQueuedTriggeredJobs();

        /// <summary>
        ///		Returns all tasks that are triggered on a scheduled timer
        /// </summary>
        IEnumerable<TriggeredJob<TimerTriggerAttribute>> GetScheduledJobs();

        /// <summary>
        ///     Returns all triggered jobs.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TriggeredJob> GetTriggeredJobs();
    }
}