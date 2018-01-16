using BoDi;
using System;
using SFA.DAS.EmployerAccounts.Events.Messages;

namespace SFA.DAS.Activities.AcceptanceTests.Steps
{
    public class TestData
    {
        public long AccountId { get; }

        private readonly IObjectContainer _objectContainer;

        private const string creatorName = "Test Environment";

        private const string creatorRef = "AT";

        public TestData(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
            AccountId = GetDateTime();
            _objectContainer.RegisterInstanceAs(PayeSchemeAddedMessage);
            _objectContainer.RegisterInstanceAs(PayeSchemeDeletedMessage);
            _objectContainer.RegisterInstanceAs(AgreementSignedMessage);
            _objectContainer.RegisterInstanceAs(UserJoinedMessage);
            _objectContainer.RegisterInstanceAs(UserInvitedMessage);
            _objectContainer.RegisterInstanceAs(LegalEntityRemovedMessage);
            _objectContainer.RegisterInstanceAs(LegalEntityAddedMessage);
            _objectContainer.RegisterInstanceAs(AccountNameChangedMessage);
            _objectContainer.RegisterInstanceAs(AccountCreatedMessage);
        }

        private long GetDateTime()
        {
            var datetime = DateTime.Now.ToString("yyyyMMddhhmmssffff");
            return long.Parse(datetime);
        }

        private AccountCreatedMessage AccountCreatedMessage => new AccountCreatedMessage(AccountId, creatorName, creatorRef);

        private AccountNameChangedMessage AccountNameChangedMessage => new AccountNameChangedMessage("Previous Test org", "Current test Org", AccountId, creatorName, creatorRef);

        private LegalEntityAddedMessage LegalEntityAddedMessage => new LegalEntityAddedMessage(AccountId, 345432, "Test Org", 321234, creatorName, creatorRef);

        private LegalEntityRemovedMessage LegalEntityRemovedMessage => new LegalEntityRemovedMessage(AccountId, 345432, false, 321234, "Test Org", creatorName, creatorRef);

        private UserInvitedMessage UserInvitedMessage => new UserInvitedMessage("Test org Admin", AccountId, creatorName, creatorRef);

        private UserJoinedMessage UserJoinedMessage => new UserJoinedMessage(AccountId, creatorName, creatorRef);

        private AgreementSignedMessage AgreementSignedMessage => new AgreementSignedMessage(AccountId, 345432, "Test org", 321234, false, creatorName, creatorRef);

        private PayeSchemeAddedMessage PayeSchemeAddedMessage => new PayeSchemeAddedMessage("123/3456", AccountId, creatorName, creatorRef);

        private PayeSchemeDeletedMessage PayeSchemeDeletedMessage => new PayeSchemeDeletedMessage("321/6543", "PayeSchemeDeletedMessageOrg", AccountId, creatorName, creatorRef);
    }
}