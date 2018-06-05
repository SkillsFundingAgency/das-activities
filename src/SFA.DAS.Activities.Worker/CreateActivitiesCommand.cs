using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Activities;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.IntegrityChecker.Worker.CreateActivities
{
    public class FakeMessage<TMessageType> : IMessage<TMessageType> 
    {
        public FakeMessage(TMessageType message, string id)
        {
            Content = message;
            Id = id;
        }

        public Task CompleteAsync()
        {
            return Task.CompletedTask;
        }

        public Task AbortAsync()
        {
            return Task.CompletedTask;
        }

        public TMessageType Content { get; }

        public string Id { get; }
    }


    class CreateActivitiesCommand
    {
        private readonly IActivitySaver _activitySaver;
        private readonly IMessageContextProvider _messageContextProvider;

        public CreateActivitiesCommand(IActivitySaver activitySaver, IMessageContextProvider messageContextProvider)
        {
            _activitySaver = activitySaver;
            _messageContextProvider = messageContextProvider;
        }

        public Task CreateActivities(int number)
        {
            var tasks = new Task[number];

            for (int i = 0; i < number; i++)
            {
                AccountCreatedMessage message = new AccountCreatedMessage(i, $"User {i}", "UserRef {i}");
                var messageContext = new FakeMessage<AccountCreatedMessage>(message, Guid.NewGuid().ToString());

                _messageContextProvider.StoreMessageContext<AccountCreatedMessage>(messageContext);
                tasks[i] = _activitySaver.SaveActivity(message, ActivityType.AccountCreated).ContinueWith(t => _messageContextProvider.ReleaseMessageContext(messageContext));
            }

            return Task.WhenAll(tasks);
        }
    }
}
