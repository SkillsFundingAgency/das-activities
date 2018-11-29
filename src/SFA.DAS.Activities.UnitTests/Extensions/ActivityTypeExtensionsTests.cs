using System;
using NUnit.Framework;
using SFA.DAS.Activities.Extensions;
using SFA.DAS.Activities.Localizers;
using SFA.DAS.Activities.Models;

namespace SFA.DAS.Activities.UnitTests.Extensions
{
    public static class ActivityTypeExtensionsTests
    {
        public class When_getting_action : Test
        {
            private ActivityType _type;
            private ActivityUrlLink _link;

            protected override void Given()
            {
                _type = ActivityType.PayeSchemeAdded;
            }

            protected override void When()
            {
                _link = _type.GetActivityLink();
            }

            [Test]
            public void Then_should_return_correct_action()
            {
                Assert.That(_link, Is.Not.Null);
                Assert.That(_link.Action, Is.EqualTo("Index"));
                Assert.That(_link.Controller, Is.EqualTo("EmployerAccountPaye"));
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