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
            return $"Account name changed from {activity.Data["PreviousName"]} to {activity.Data["CurrentName"]} by {activity.Data["CreatorName"]}";
        }
    }
}