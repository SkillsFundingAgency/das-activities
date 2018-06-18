using Microsoft.Azure.WebJobs;

namespace SFA.DAS.Activities.Jobs.Infrastructure
{
    public interface IJobHostFactory
    {
        JobHost CreateJobHost();
    }
}