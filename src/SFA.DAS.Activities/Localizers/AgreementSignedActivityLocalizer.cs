namespace SFA.DAS.Activities.Localizers
{
    public class AgreementSignedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} agreements signed";
        }

        public string GetSingularText(Activity activity)
        {
            return $"Agreement signed for {activity.Data["OrganisationName"]} by {activity.Data["CreatorName"]}";
        }
    }
}