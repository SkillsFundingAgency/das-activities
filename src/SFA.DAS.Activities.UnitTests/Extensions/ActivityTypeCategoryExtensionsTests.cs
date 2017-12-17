using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.UnitTests.Extensions
{
    public static class ActivityTypeCategoryExtensionsTests
    {
        public class When_getting_activity_types : Test
        {
            private ActivityTypeCategory _category;
            private List<ActivityType> _activityTypes;

            protected override void Given()
            {
                _category = ActivityTypeCategory.Unknown;
            }

            protected override void When()
            {
                _activityTypes = _category.GetActivityTypes();
            }

            [Test]
            public void Then_should_return_correct_activity_types()
            {
                Assert.That(_activityTypes, Is.Not.Null);
                Assert.That(_activityTypes.Count, Is.EqualTo(1));
                Assert.That(_activityTypes, Does.Contain(ActivityType.Unknown));
            }
        }
    }
}