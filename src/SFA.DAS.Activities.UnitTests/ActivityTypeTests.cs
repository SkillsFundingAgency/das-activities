using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SFA.DAS.Activities.Attributes;
using CategoryAttribute = SFA.DAS.Activities.Attributes.CategoryAttribute;

namespace SFA.DAS.Activities.UnitTests
{
    public static class ActivityTypeTests
    {
        public class When_activity_types_are_defined : Test
        {
            private Type _activityType;
            private List<FieldInfo> _activityTypes;

            protected override void Given()
            {
                _activityType = typeof(ActivityType);
            }

            protected override void When()
            {
                _activityTypes = Enum.GetValues(_activityType)
                    .Cast<ActivityType>()
                    .Select(t => t.GetType().GetField(t.ToString()))
                    .ToList();
            }

            [Test]
            public void Then_activity_type_categories_should_be_defined()
            {
                foreach (var activityType in _activityTypes)
                {
                    Assert.That(activityType.GetCustomAttribute<CategoryAttribute>(), Is.Not.Null);
                }
            }

            [Test]
            public void Then_activity_type_localizers_should_be_defined()
            {
                foreach (var activityType in _activityTypes)
                {
                    Assert.That(activityType.GetCustomAttribute<LocalizerAttribute>(), Is.Not.Null);
                }
            }
        }
    }
}