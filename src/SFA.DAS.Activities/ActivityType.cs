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
        LegalEntityAdded = 6,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(AccountNameChangedActivityLocalizer))]
        AccountNameChanged = 7,

        [Category(ActivityTypeCategory.AccountAdmin)]
        [Localizer(typeof(UserInvitedActivityLocalizer))]
        UserInvited = 8
    }
}