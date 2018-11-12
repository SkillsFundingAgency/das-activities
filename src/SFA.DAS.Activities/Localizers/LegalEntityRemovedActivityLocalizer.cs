using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("{0} removed by {1}", "OrganisationName", "CreatorName");
        }
    }
}