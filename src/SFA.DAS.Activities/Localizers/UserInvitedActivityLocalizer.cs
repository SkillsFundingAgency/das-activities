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
            return $"{activity.Data["PersonInvited"]} invited by {activity.Data["CreatorName"]}";
        }
    }
}