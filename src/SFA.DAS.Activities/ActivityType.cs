using SFA.DAS.Activities.Attributes;
using SFA.DAS.Activities.Localizers;

namespace SFA.DAS.Activities
{
    public enum ActivityType
    {
        [Category(ActivityTypeCategory.Unknown)]
        [Localizer(typeof(UnknownActivityLocalizer))]
        Unknown = 0,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(PayeSchemeAddedActivityLocalizer))]
        [Action("Index", "EmployerAccountPaye")]
        PayeSchemeAdded = 1,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(PayeSchemeRemovedActivityLocalizer))]
        [Action("Index", "EmployerAccountPaye")]
        PayeSchemeRemoved = 2,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(AccountCreatedActivityLocalizer))]
        AccountCreated = 3,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(UserJoinedActivityLocalizer))]
        [Action("Index", "EmployerTeam")]
        UserJoined = 4,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(LegalEntityRemovedActivityLocalizer))]
        LegalEntityRemoved = 5,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(LegalEntityAddedActivityLocalizer))]
        [Action("Index", "EmployerAgreement")]
        LegalEntityAdded = 6,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(AccountNameChangedActivityLocalizer))]
        [Action("Index", "EmployerTeam")]
        AccountNameChanged = 7,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(UserInvitedActivityLocalizer))]
        [Action("ViewTeam", "EmployerTeam")]
        UserInvited = 8,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(AgreementSignedActivityLocalizer))]
        [Action("Index", "EmployerAgreement")]
        AgreementSigned = 9,

        [Category(ActivityTypeCategory.Payments)]
        [Localizer(typeof(PaymentCreatedActivityLocalizer))]
        [Action("Index", "EmployerAccountTransactions")]
        PaymentCreated = 10
    }
}