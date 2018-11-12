using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("PAYE scheme {0} removed by {1}", "PayeScheme", "CreatorName");
        }
    }
}