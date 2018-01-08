namespace SFA.DAS.Activities.Localizers
{
    public class PayeSchemeRemovedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} PAYE schemes removed";
        }

        public string GetSingularText(Activity activity)
        {
            return $"PAYE scheme {activity.Data["PayeScheme"]} removed by {activity.Data["CreatorName"]}";
        }
    }
}