using System;
using System.Threading;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Worker.Services;

namespace SFA.DAS.Activities.UnitTests.Worker.Services
{
    public static class ActivitiesServiceTests
    {
        public class When_adding_an_activity: Test
        {
            private IActivitiesService _service;
            private readonly Activity _activity = new Activity();
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();

            protected override void Given()
            {
                _service = new ActivitiesService(_client.Object);
            }

            protected override void When()
            {
                _service.AddActivity(_activity);
            }

            [Test]
            public void Then_should_index_activity()
            {
                _client.Verify(c => c.IndexAsync(_activity, It.IsAny<Func<IndexDescriptor<Activity>, IIndexRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}