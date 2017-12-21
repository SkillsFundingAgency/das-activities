using System;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.ObjectMappers;

namespace SFA.DAS.Activities.UnitTests.Worker.ObjectMappers
{
    public static class ActivityMapperTests
    {
        public class When_mapping_from_an_event_to_an_activity_with_explicit_id_and_at_properties : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEvent _event = new StubEvent
            {
                AccountId = 123,
                CreatedAt = DateTime.UtcNow,
                Foo = "Bar"
            };

            private Activity _activity;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                _activity = _mapper.Map(_event, ActivityType.PayeSchemeAdded, e => e.AccountId, e => e.CreatedAt);
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_activity, Is.Not.Null);
                Assert.That(_activity.Type, Is.EqualTo(ActivityType.PayeSchemeAdded));
                Assert.That(_activity.AccountId, Is.EqualTo(_event.AccountId));
                Assert.That(_activity.At, Is.EqualTo(_event.CreatedAt));
                Assert.That(_activity.Data, Is.Not.Null);
                Assert.That(_activity.Data.Count, Is.EqualTo(1));
                Assert.That(_activity.Data.TryGetValue("Foo", out var foo), Is.True);
                Assert.That(foo, Is.EqualTo(_event.Foo));
            }
        }

        public class When_mapping_from_an_event_to_an_activity_with_implicit_id_and_at_properties : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEvent _event = new StubEvent
            {
                AccountId = 123,
                CreatedAt = DateTime.UtcNow,
                Foo = "Bar"
            };

            private Activity _activity;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                _activity = _mapper.Map(_event, ActivityType.PayeSchemeAdded);
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_activity, Is.Not.Null);
                Assert.That(_activity.Type, Is.EqualTo(ActivityType.PayeSchemeAdded));
                Assert.That(_activity.AccountId, Is.EqualTo(_event.AccountId));
                Assert.That(_activity.At, Is.EqualTo(_event.CreatedAt));
                Assert.That(_activity.Data, Is.Not.Null);
                Assert.That(_activity.Data.Count, Is.EqualTo(1));
                Assert.That(_activity.Data.TryGetValue("Foo", out var foo), Is.True);
                Assert.That(foo, Is.EqualTo(_event.Foo));
            }
        }

        public class When_mapping_from_an_event_with_null_AccountId_property_to_an_activity : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEventWithNullProps _event = new StubEventWithNullProps
            {
                AccountId = null,
                CreatedAt = DateTime.UtcNow
            };
            
            private Exception _ex;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                try
                {
                    _mapper.Map(_event, ActivityType.PayeSchemeAdded);
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Is.EqualTo("'AccountId' cannot be null."));
            }
        }

        public class When_mapping_from_an_event_with_null_CreatedAt_property_to_an_activity : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEventWithNullProps _event = new StubEventWithNullProps
            {
                AccountId = 123,
                CreatedAt = null
            };

            private Exception _ex;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                try
                {
                    _mapper.Map(_event, ActivityType.PayeSchemeAdded);
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Is.EqualTo("'CreatedAt' cannot be null."));
            }
        }

        public class When_mapping_from_an_event_without_an_AccountId_property_to_an_activity : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEventWithNoAccountIdProp _event = new StubEventWithNoAccountIdProp
            {
                CreatedAt = DateTime.UtcNow
            };

            private Exception _ex;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                try
                {
                    _mapper.Map(_event, ActivityType.PayeSchemeAdded);
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Is.EqualTo("Could not find an 'AccountId' property."));
            }
        }

        public class When_mapping_from_an_event_without_a_CreatedAt_property_to_an_activity : Test
        {
            private IActivityMapper _mapper;

            private readonly StubEventWithNoCreatedAtProp _event = new StubEventWithNoCreatedAtProp
            {
                AccountId = 123
            };

            private Exception _ex;

            protected override void Given()
            {
                _mapper = new ActivityMapper();
            }

            protected override void When()
            {
                try
                {
                    _mapper.Map(_event, ActivityType.PayeSchemeAdded);
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
            }

            [Test]
            public void Then_should_create_activity()
            {
                Assert.That(_ex, Is.Not.Null);
                Assert.That(_ex.Message, Is.EqualTo("Could not find a 'CreatedAt' property."));
            }
        }

        private class StubEvent
        {
            public long AccountId { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Foo { get; set; }
        }

        private class StubEventWithNullProps
        {
            public long? AccountId { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        private class StubEventWithNoAccountIdProp
        {
            public DateTime CreatedAt { get; set; }
        }

        private class StubEventWithNoCreatedAtProp
        {
            public long AccountId { get; set; }
        }
    }
}