using System.Threading.Tasks;

namespace SFA.DAS.Activities.AcceptanceTests.Azure
{
    public interface IAzureTopicMessageBus
    {
        Task PublishAsync(object message);
    }
}
