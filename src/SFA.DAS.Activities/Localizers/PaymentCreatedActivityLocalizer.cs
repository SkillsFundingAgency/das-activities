namespace SFA.DAS.Activities.Localizers
{
    public class PaymentCreatedActivityLocalizer : IActivityLocalizer
    {
        public string GetPluralText(Activity activity, long count)
        {
            return $"{count} payments made";
        }

        public string GetSingularText(Activity activity)
        {

            return $"£{activity.Data["Amount"]} payment made to {activity.Data["ProviderName"]}";
        }
    }
}