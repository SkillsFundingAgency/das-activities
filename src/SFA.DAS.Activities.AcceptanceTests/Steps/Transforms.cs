using System;
using SFA.DAS.EmployerAccounts.Events.Messages;
using TechTalk.SpecFlow;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    [Binding]
    public class Transforms
    {
        [StepArgumentTransformation(@"(\d+) days ago?")]
        public DateTime DaysAgoTramsform(int p0)
        {
            return DateTime.UtcNow.Date.AddHours(12).AddDays(-p0);
        }

        [StepArgumentTransformation(@"([^ ]*) message?")]
        public Type MessageTransform(string p0)
        {
            var messageBaseType = typeof(AccountMessageBase);
            var messageType = Type.GetType($"{messageBaseType.Namespace}.{p0}, {messageBaseType.Assembly.FullName}");

            return messageType;
        }
    }
}