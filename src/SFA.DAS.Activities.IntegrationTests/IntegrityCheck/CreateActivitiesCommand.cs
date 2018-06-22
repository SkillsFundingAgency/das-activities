using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.IntegrationTests.IntegrityCheck
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
}
