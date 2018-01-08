namespace SFA.DAS.Activities.Localizers
{
    public class LegalEntityRemovedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} organisations removed";
        }

        public string GetSingularText(Activity activity)
        {
            return $"{activity.Data["OrganisationName"]} removed by {activity.Data["CreatorName"]}";
        }
    }
}