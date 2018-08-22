using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    public class AzureWebJobHelper : IAzureWebJobHelper
    {
        private readonly ITriggeredJobRepository _triggeredJobRepository;
        private readonly IAzureQueueClient _azureQueueClient;
        private readonly ILog _logger;

        public AzureWebJobHelper(ITriggeredJobRepository triggeredJobRepository, IAzureQueueClient azureQueueClient, ILog logger)
        {
            _triggeredJobRepository = triggeredJobRepository;
            _azureQueueClient = azureQueueClient;
            _logger = logger;
        }

        public void EnsureAllQueuesForTriggeredJobs()
        {
            foreach (var triggeredJob in _triggeredJobRepository.GetQueuedTriggeredJobs())
            {
                _azureQueueClient.EnsureQueueExistsAsync(triggeredJob.Trigger.QueueName);
            }
        }
    }
}