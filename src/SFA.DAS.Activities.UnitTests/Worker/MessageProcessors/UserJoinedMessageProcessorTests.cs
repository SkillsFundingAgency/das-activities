using NUnit.Framework;
using SFA.DAS.Activities.Worker.MessageProcessors;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.UnitTests.Worker.MessageProcessors
{
    public class UserJoinedMessageProcessorTests : MessageProcessorTest<UserJoinedMessageProcessor>
    {
        [Test]
        public void When_processing_UserJoinedMessage_then_should_index_UserJoined_activity()
        {
            From(new UserJoinedMessage()).To(ActivityType.UserJoined).Verify();
        }
    }
}