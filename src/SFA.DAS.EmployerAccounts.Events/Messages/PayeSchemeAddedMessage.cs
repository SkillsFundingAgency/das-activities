using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeAddedMessage
    {
        public string PayeScheme { get; set; }
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorName { get; set; }
        public string CreatorUserRef { get; set; }
    }
}