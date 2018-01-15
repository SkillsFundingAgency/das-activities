using BoDi;
using System;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class TestData
    {
        public long AccountId { get; }

        private readonly IObjectContainer _objectContainer;

        public TestData(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
            AccountId = GetDateTime();
            _objectContainer.RegisterInstanceAs(PayeSchemeAddedMessage);
            _objectContainer.RegisterInstanceAs(PayeSchemeDeletedMessage);
        }

        private long GetDateTime()
        {
            var datetime = DateTime.Now.ToString("yyyyMMddhhmmssffff");
            return long.Parse(datetime);
        }

        private PayeSchemeAddedMessage PayeSchemeAddedMessage => new PayeSchemeAddedMessage("123/3456", AccountId, "Test", "ABC");

        private PayeSchemeDeletedMessage PayeSchemeDeletedMessage => new PayeSchemeDeletedMessage("321/6543", "PayeSchemeDeletedMessageOrg", AccountId, "Test", "ABC");
    }
}