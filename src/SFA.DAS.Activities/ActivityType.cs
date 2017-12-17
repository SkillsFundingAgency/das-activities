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
        PayeSchemeRemoved = 2
    }
}