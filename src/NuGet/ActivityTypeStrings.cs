//using Nest;

namespace NuGet
{
        public static class ActivityTypeStrings
        {
            public const string UserJoined = "UserJoined";
            public const string UserInvited = "UserInvited";
            public const string AccountCreated = "AccountCreated";
            public const string PayeSchemeCreated = "PayeSchemeCreated";
            public const string PayeSchemeDeleted = "PayeSchemeDeleted";
            public const string AgreementCreated = "AgreementCreated";
            public const string AgreementSigned = "AgreementSigned";
            public const string AccountNameChanged = "AccountNameChanged";
            public const string LevyPaymentRecieved = "LevyPaymentRecieved";

            //not required for release
            public const string LegalEntityRemoved = "LegalEntityRemoved";
        } 
}
