using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Localizers
{
    public class AccountNameChangedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} account names changed";
        }

        public string GetSingularText(Activity activity)
        {
            return activity.GetMessageForActivity("Account name changed from {0} to {1} by {2}", "PreviousName", "CurrentName", "CreatorName");
        }
    }
}