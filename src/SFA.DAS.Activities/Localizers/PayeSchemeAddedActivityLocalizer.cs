namespace SFA.DAS.Activities.Localizers
{
    public class PayeSchemeAddedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} PAYE schemes added";
        }

        public string GetSingularText(Activity activity)
        {
            return $"PAYE scheme {activity.Data["PayeScheme"]} added by {activity.Data["CreatorName"]}";
        }
    }
}