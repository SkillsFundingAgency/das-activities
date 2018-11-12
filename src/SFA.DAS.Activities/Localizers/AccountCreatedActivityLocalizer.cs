using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Localizers
{
    public class AccountCreatedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} accounts created";
        }

        public string GetSingularText(Activity activity)
        {
            return activity.GetMessageForActivity("Account created by {0}", "CreatorName");
        }
    }
}