using System;
using NUnit.Framework;
using SFA.DAS.Activities.Localizers;
using System.Collections.Generic;

namespace SFA.DAS.Activities.UnitTests.Localizers
{
    [TestFixture]
    public class PaymentCreatedActivityLocalizerTest
    {
        [TestCase("5614578.00", "£5,614,578.00")]
        [TestCase("14578.00", "£14,578.00")]
        [TestCase("0", "£0.00")]
        [TestCase("8.00", "£8.00")]
        [TestCase("18", "£18.00")]
        [TestCase("20.5", "£20.50")]
        [TestCase("120.55", "£120.55")]
        [TestCase("2120.55", "£2,120.55")]

        public void When_reading_PaymentCreatedActivity_then_should_format_currency(string amount, string expected)
        {
            //Arrange
            var paymentCreatedActivityLocalizer = new PaymentCreatedActivityLocalizer();

            var data = new Dictionary<string, string>
            {
                { "Amount", amount },
                { "ProviderName", "Test provider"}
            };

            //Act
            var activity = paymentCreatedActivityLocalizer.GetSingularText(new Activity { AccountId = 4578, At = DateTime.Now, Created = DateTime.Now, Type = ActivityType.PaymentCreated, Data = data });

            //Assert
            StringAssert.StartsWith(expected, activity, activity);
        }
    }
}
