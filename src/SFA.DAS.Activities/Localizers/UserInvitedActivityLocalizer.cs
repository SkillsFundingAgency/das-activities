using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Localizers
{
    public class UserInvitedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} users invited";
        }

        public string GetSingularText(Activity activity)
        {
            return activity.GetMessageForActivity("{0} invited by {1}", "PersonInvited", "CreatorName");
        }
    }
}