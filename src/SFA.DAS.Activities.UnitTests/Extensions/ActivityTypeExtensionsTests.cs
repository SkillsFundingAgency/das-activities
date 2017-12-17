using NUnit.Framework;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.UnitTests.Extensions
{
    public static class ActivityTypeExtensionsTests
    {
        public class When_getting_description : Test
        {
            private ActivityType _type;
            private string _description;

            protected override void Given()
            {
                _type = ActivityType.PayeSchemeAdded;
            }

            protected override void When()
            {
                _description = _type.GetDescription();
            }

            [Test]
            public void Then_should_return_correct_description()
            {
                Assert.That(_description, Is.EqualTo("Paye Scheme Added"));
            }
        }
    }
}