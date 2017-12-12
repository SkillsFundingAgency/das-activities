using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.Client.Elastic;

namespace SFA.DAS.Activities.UnitTests.Client.Elastic
{
    public static class IndexAutoMapperTests
    {
        public class When_ensuring_index_exists_more_than_once : TestAsync
        {
            private const string IndexName = "activities";

            private IIndexAutoMapper _indexAutoMapper;
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();
            private readonly Mock<IExistsResponse> _indexExistsResponse = new Mock<IExistsResponse>();

            protected override void Given()
            {
                _indexExistsResponse.Setup(i => i.Exists).Returns(false);

                _client.Setup(c => c.IndexExistsAsync(IndexName, It.IsAny<Func<IndexExistsDescriptor, IIndexExistsRequest>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_indexExistsResponse.Object);

                _indexAutoMapper = new IndexAutoMapper(_client.Object);
            }

            protected override async Task When()
            {
                await _indexAutoMapper.EnureIndexExists<Activity>(IndexName);
                await _indexAutoMapper.EnureIndexExists<Activity>(IndexName);
            }

            [Test]
            public void Then_should_create_activity_index_once()
            {
                _client.Verify(c => c.CreateIndexAsync(IndexName, It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}