using SFA.DAS.Activities.Client.Attributes;

namespace SFA.DAS.Activities.Client
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