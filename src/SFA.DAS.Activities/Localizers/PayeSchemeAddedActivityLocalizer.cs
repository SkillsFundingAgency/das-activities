using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("PAYE scheme {0} added by {1}", "PayeScheme", "CreatorName");
        }
    }
}