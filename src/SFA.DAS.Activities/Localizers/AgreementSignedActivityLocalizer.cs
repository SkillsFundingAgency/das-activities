using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("Agreement signed for {0} by {1}", "OrganisationName", "CreatorName");
        }
    }
}