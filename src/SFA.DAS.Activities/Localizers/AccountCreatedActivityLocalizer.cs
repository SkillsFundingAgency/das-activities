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
            return $"Account created by {activity.Data["CreatorName"]}";
        }
    }
}