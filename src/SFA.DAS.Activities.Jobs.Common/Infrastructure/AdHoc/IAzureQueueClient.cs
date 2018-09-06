using System.Threading.Tasks;

namespace SFA.DAS.Activities.Jobs.Common.Infrastructure.AdHoc
{
    /// <summary>
    ///		Represents a service for managing the existence of Azure queues.
    /// </summary>
    public interface IAzureQueueClient
    {
        Task EnsureQueueExistsAsync(string queueName);

        void QueueMessage<TMessage>(string queueName, TMessage message);
    }
}