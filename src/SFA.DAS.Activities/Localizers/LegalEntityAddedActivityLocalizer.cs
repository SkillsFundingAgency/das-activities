namespace SFA.DAS.Activities.Localizers
{
    public class LegalEntityAddedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} organisations added";
        }

        public string GetSingularText(Activity activity)
        {
            return $"{activity.Data["OrganisationName"]} added by {activity.Data["CreatorName"]}";
        }
    }
}