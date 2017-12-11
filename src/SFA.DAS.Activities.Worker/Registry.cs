using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Activities.Worker
{
    public class Registry : StructureMap.Registry
    {
        public Registry()
        {
            For<IMessageProcessor>().Use<PayeSchemeCreatedMessageProcessor>().Named("PayeSchemeCreatedMessageProcessor");
        }
    }
}