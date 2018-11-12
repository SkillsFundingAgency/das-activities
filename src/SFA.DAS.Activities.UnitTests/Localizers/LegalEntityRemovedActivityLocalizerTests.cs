using System;
using NUnit.Framework;
using SFA.DAS.Activities.Localizers;
using System.Collections.Generic;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.UnitTests.Localizers
{
    [TestFixture]
    public class LegalEntityRemovedActivityLocalizerTests
    {
        [Test]
        public void GetSingularText_RequiredDataItemsArePresent_ShouldGetFullMessage()
        {
            //Arrange
            var localizer = new LegalEntityRemovedActivityLocalizer();

            var data = new Dictionary<string, string>
            {
                { "OrganisationName", "Acme Fireworks" },
                { "CreatorName", "Wile E. Coyote"}
            };

            //Act
            var actualMessage = localizer.GetSingularText(new Activity { Data = data });

            //Assert
            const string expectedMessage = "Acme Fireworks removed by Wile E. Coyote";

            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void GetSingularText_RequiredDataItemsAreNotPresent_ShouldGetGenericMessage()
        {
            //Arrange
            var localizer = new LegalEntityRemovedActivityLocalizer();

            var data = new Dictionary<string, string>();

            //Act
            var activity = new Activity {Data = data, Type = ActivityType.LegalEntityRemoved};
            var actualMessage = localizer.GetSingularText(activity);

            //Assert
            string expectedMessage = activity.GetGenericMessage();

            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}
