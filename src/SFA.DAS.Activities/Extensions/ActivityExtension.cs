using SFA.DAS.Activities.Models;


namespace SFA.DAS.Activities.Extensions
{
    public static class ActivityExtension
    {
        public static ActivityUrlLink GetDetailsLink(this Activity activity)
        {
            var actionDetails = activity.Type.GetAction();

            if (actionDetails == null)
                return null;

            var link = new ActivityUrlLink
            {
                Controller = actionDetails.Item2,
                Action = actionDetails.Item1
            };

            if (activity.Type == ActivityType.PaymentCreated)
            {
                link.Parameters.Add("month", activity.At.Month);
                link.Parameters.Add("year", activity.At.Year);
            }

            return link;
        }
    }
}
