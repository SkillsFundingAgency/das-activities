using System;
using System.Globalization;

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
            var amount = string.Format(new CultureInfo("en-GB"), "{0:C2}", Convert.ToDecimal(activity.Data["Amount"]));
            return $"{amount} payment made to {activity.Data["ProviderName"]}";
        }
    }
}