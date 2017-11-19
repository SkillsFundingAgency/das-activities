using System;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    [MessageGroup("add_paye_scheme")]
    public class PayeSchemeCreatedMessage
    {
        public string EmpRef { get; set; }
        public long AccountId { get; set; }
        public DateTime PostedDatedTime { get; set; }
        public string CreatedByName { get; set; }
    }
}