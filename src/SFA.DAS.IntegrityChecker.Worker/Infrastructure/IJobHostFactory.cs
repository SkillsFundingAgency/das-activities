using Microsoft.Azure.WebJobs;

namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public interface IJobHostFactory
    {
        JobHost CreateJobHost();
    }
}