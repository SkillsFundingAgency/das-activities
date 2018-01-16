using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.Helpers;

namespace SFA.DAS.Activities.AcceptanceTests.Azure
{
    public class AzureTopicMessageBus : IAzureTopicMessageBus
    {
        private readonly string _manageApprenticeshipsServiceBusConnectionString;
        private readonly string _commitmentsServiceBusConnectionString;        

        public AzureTopicMessageBus(string manageApprenticeshipsServiceBusConnectionString, string commitmentsServiceBusConnectionString)
        {
            _manageApprenticeshipsServiceBusConnectionString = manageApprenticeshipsServiceBusConnectionString;
            _commitmentsServiceBusConnectionString = commitmentsServiceBusConnectionString;
        }

        public async Task PublishAsync(object message)
        {
            var topicName = MessageGroupHelper.GetMessageGroupName(message);
            var connectionString = GetConnectionString(message);

            TopicClient client = null;

            try
            {
                client = TopicClient.CreateFromConnectionString(connectionString, topicName);
                await client.SendAsync(new BrokeredMessage(message));
            }
            finally
            {
                if (client != null && !client.IsClosed)
                {
                    await client.CloseAsync();
                }
            }
        }

        private string GetConnectionString(object message)
        {
            return message.GetType().FullName.Contains("EmployerAccounts")
                ? _manageApprenticeshipsServiceBusConnectionString
                : _commitmentsServiceBusConnectionString;
        }
    }
}