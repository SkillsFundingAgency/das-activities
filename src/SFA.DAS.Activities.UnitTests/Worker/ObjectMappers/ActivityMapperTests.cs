using System;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.ObjectMappers;

namespace SFA.DAS.Activities.UnitTests.Worker.ObjectMappers
{
    public static class ActivityMapperTests
    {
        public class When_mapping : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEvent _event = new StubEvent
            {
                AccountId = 123,
                At = DateTime.UtcNow,
                Foo = "Bar"
            };

            private Activity _activity;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                _activity = _mapper.Map(_event, ActivityType.PayeSchemeAdded, e => e.AccountId, e => e.At);
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_activity, Is.Not.Null);
                Assert.That(_activity.Type, Is.EqualTo(ActivityType.PayeSchemeAdded));
                Assert.That(_activity.AccountId, Is.EqualTo(_event.AccountId));
                Assert.That(_activity.At, Is.EqualTo(_event.At));
                Assert.That(_activity.Data, Is.Not.Null);
                Assert.That(_activity.Data["Foo"], Is.EqualTo(_event.Foo));
            }
        }

        private class StubEvent
        {
            public long AccountId { get; set; }
            public DateTime At { get; set; }
            public string Foo { get; set; }
        }
    }
}