using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    public class AdHocJobs
    {
        private static readonly ILog Log = new NLogLogger(typeof(AdHocJobs));

        public void ProcessQueueMessage(
            [QueueTrigger(Constants.AzureQueueNames.AdHocJobQueue)] AdHocJobParams jobParams, 
            TraceWriter logger,
            CancellationToken cancellationToken)
        {
            logger.Info($"Received request to process job type {jobParams.JobName}");
            var triggeredJob = FindTriggeredJob(jobParams);;

            if (triggeredJob != null)
            {
                RunJob(triggeredJob, logger, cancellationToken);
            }
        }

        public TriggeredJob FindTriggeredJob(AdHocJobParams jobParams)
        {
            var jobName = new JobName(jobParams.JobName);

            var possibleJobs = GetAllTriggeredJobs()
                                    .Where(tj => MatchesOnClassAndMethod(jobName, tj) || MatchesOnClass(jobName, tj) || MatchesOnMethod(jobName, tj))
                                    .ToArray();

            if(possibleJobs.Length == 1)
            {
                return possibleJobs[0];
            }

            if (possibleJobs.Length == 0)
            {
                Log.Warn($"A request to run {jobName.FullName} failed because the specified job could not be found - available jobs are {BuildAvailableJobMessages()}");
            }

            if(possibleJobs.Length > 0)
            {
                Log.Warn($"A request to run {jobName.FullName} failed because more than one specified jobs could not be found - available jobs are {BuildAvailableJobMessages()}. Of these {BuildPossibleJobMessages(possibleJobs)} were matched.");
            }

            return null;
        }

        private string BuildAvailableJobMessages()
        {
            return BuildPossibleJobMessages(GetAllTriggeredJobs());
        }

        private string BuildPossibleJobMessages(IEnumerable<TriggeredJob> possibleJobs)
        {
            return string.Join(",", possibleJobs.Select(pj => $"{pj.ContainingClass.FullName}.{pj.InvokedMethod.Name}"));
        }

        private bool MatchesOnClassAndMethod(JobName jobName, TriggeredJob job)
        {
            // Caller supplied class.method, so unambiguous
            return jobName.HasClassName
                    && jobName.HasMethodName
                    && job.ContainingClass.Name.Equals(jobName.ClassName, StringComparison.InvariantCultureIgnoreCase)
                    && job.InvokedMethod.Name.Equals(jobName.MethodName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool MatchesOnClass(JobName jobName, TriggeredJob job)
        {
            // Caller supplied only a name - could be class or method
            return !jobName.HasClassName
                    && !jobName.HasMethodName
                    && job.ContainingClass.Name.Equals(jobName.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool MatchesOnMethod(JobName jobName, TriggeredJob job)
        {
            // Caller supplied only a name - could be class or method
            return !jobName.HasClassName 
                    && !jobName.HasMethodName
                    && job.InvokedMethod.Name.Equals(jobName.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        private IEnumerable<TriggeredJob> GetAllTriggeredJobs()
        {
            var triggeredJobsRepo = ServiceLocator.Get<ITriggeredJobRepository>();

            return triggeredJobsRepo.GetTriggeredJobs();
        }

        private void RunJob(TriggeredJob jobtype, TraceWriter logger, CancellationToken cancellationToken)
        {
            using (var thisContainer = ServiceLocator.CreateNestedContainer())
            {
                logger.Info($"found job type {jobtype.FullName}");
                try
                {
                    var job = thisContainer.GetInstance(jobtype.ContainingClass);
                    logger.Verbose($"Obtained instance of type {jobtype.FullName} from IoC");

                    var methodParamValues = BuildParams(jobtype, logger, cancellationToken);

                    jobtype.InvokedMethod.Invoke(job, methodParamValues);

                    Task.Run(() => jobtype.InvokedMethod.Invoke(job, methodParamValues))
                        .ContinueWith(task =>
                                {
                                    logger.Info($"Job has ended. Cancelled?:{task.IsCanceled} Faulted?:{task.IsFaulted}");
                                    if (task.IsFaulted)
                                    {
                                        logger.Error($"Exception: {task.Exception}");
                                    }
                                });

                }
                catch (Exception e)
                {
                    logger.Error($"Failed to fetch and execute instance of type {jobtype.FullName} from IoC - error: {e.GetType()} - {e.Message}");
                    throw;
                }
            }
        }

        private object[] BuildParams(TriggeredJob triggeredJob, params object[] values)
        {
            var methodParams = triggeredJob.InvokedMethod.GetParameters();

            var result = new object[methodParams.Length];

            for (int i = 0; i < methodParams.Length; i++)
            {
                var methodParam = methodParams[i];
                result[i] = values.FirstOrDefault(v => v != null && v.GetType() == methodParam.ParameterType);
            }

            return result;
        }
    }
}