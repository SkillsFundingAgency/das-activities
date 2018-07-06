using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure
{
    public interface IJobHostFactory
    {
        JobHost CreateJobHost();
    }
}