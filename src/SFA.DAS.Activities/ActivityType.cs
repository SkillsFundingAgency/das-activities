using SFA.DAS.Activities.Attributes;

namespace SFA.DAS.Activities
{
    public enum ActivityType
    {
        [Category(ActivityTypeCategory.Unknown)]
        Unknown = 0,
        
        [Category(ActivityTypeCategory.AccountAdmin)]
        PayeSchemeAdded = 1,
        
        [Category(ActivityTypeCategory.AccountAdmin)]
        PayeSchemeRemoved = 2,

        [Category(ActivityTypeCategory.AccountAdmin)]
        AccountCreated = 3,

        [Category(ActivityTypeCategory.AccountAdmin)]
        UserJoined = 4,

        [Category(ActivityTypeCategory.AccountAdmin)]
        LegalEntityRemoved = 5,

        [Category(ActivityTypeCategory.AccountAdmin)]
        LegalEntityAdded = 6,

        [Category(ActivityTypeCategory.AccountAdmin)]
        AccountNameChanged = 7,
    }
}