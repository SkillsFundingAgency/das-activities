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
            _objectContainer =objectContainer;
            AccountId = new Random().Next(10000, 99999);
            _objectContainer.RegisterInstanceAs(PayeSchemeAddedMessage,"PayeSchemeAddedMessage");
        }

        private PayeSchemeAddedMessage PayeSchemeAddedMessage => new PayeSchemeAddedMessage("123/3456", AccountId, "Test", "ABC");
    }
}