using System;
using System.Globalization;
using SFA.DAS.Activities.Extensions;

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
            return activity.GetMessageForActivity("{0} payment made to {1}", FormatCurrency, "Amount", "ProviderName");
        }

        private string FormatCurrency(string dataItemName, string dataItemValue)
        {
            if (string.Equals(dataItemName, "amount", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format(new CultureInfo("en-GB"), "{0:C2}", Convert.ToDecimal(dataItemValue));
            }

            return dataItemValue;
        }
    }
}