using System;
using NUnit.Framework;
using SFA.DAS.Activities.Extensions;
using SFA.DAS.Activities.Localizers;

namespace SFA.DAS.Activities.UnitTests.Extensions
{
    public static class ActivityTypeExtensionsTests
    {
        public class When_getting_action : Test
        {
            private ActivityType _type;
            private Tuple<string, string> _action;

            protected override void Given()
            {
                _type = ActivityType.PayeSchemeAdded;
            }

            protected override void When()
            {
                _action = _type.GetAction();
            }

            [Test]
            public void Then_should_return_correct_action()
            {
                Assert.That(_action, Is.Not.Null);
                Assert.That(_action.Item1, Is.EqualTo("Index"));
                Assert.That(_action.Item2, Is.EqualTo("EmployerAccountPaye"));
            }
        }

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

        public class When_getting_localizer : Test
        {
            private ActivityType _type;
            private IActivityLocalizer _localizer;

            protected override void Given()
            {
                _type = ActivityType.PayeSchemeAdded;
            }

            protected override void When()
            {
                _localizer = _type.GetLocalizer();
            }

            [Test]
            public void Then_should_return_correct_localizer()
            {
                Assert.That(_localizer, Is.Not.Null);
                Assert.That(_localizer, Is.TypeOf<PayeSchemeAddedActivityLocalizer>());
            }
        }
    }
}