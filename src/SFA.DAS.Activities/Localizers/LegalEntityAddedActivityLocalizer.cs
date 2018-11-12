using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("{0} added by {1}", "OrganisationName", "CreatorName");
        }
    }
}