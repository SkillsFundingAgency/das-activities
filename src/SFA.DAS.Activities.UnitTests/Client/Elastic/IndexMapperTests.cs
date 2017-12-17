﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Client.Elastic;

namespace SFA.DAS.Activities.UnitTests.Client.Elastic
{
    public static class IndexMapperTests
    {
        private const string StubIndexName = "stubs";

        public class When_ensuring_non_existant_index_exists : TestAsync
        {
            private StubIndexMapper _indexMapper;
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();
            private readonly Mock<IExistsResponse> _indexExistsResponse = new Mock<IExistsResponse>();
            private readonly Mock<IConnectionSettingsValues> _connectionSettings = new Mock<IConnectionSettingsValues>();
            private readonly FluentDictionary<Type, string> _defaultIndices = new FluentDictionary<Type, string>();
            private readonly CreateIndexDescriptor _indexDescriptor = new CreateIndexDescriptor(StubIndexName);
            private readonly Mock<ICreateIndexResponse> _createIndexReponse = new Mock<ICreateIndexResponse>();

            protected override void Given()
            {
                _connectionSettings.Setup(s => s.DefaultIndices).Returns(_defaultIndices);

                _indexExistsResponse.Setup(i => i.Exists).Returns(false);

                _client.Setup(c => c.ConnectionSettings).Returns(_connectionSettings.Object);

                _client.Setup(c => c.IndexExistsAsync(StubIndexName, It.IsAny<Func<IndexExistsDescriptor, IIndexExistsRequest>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_indexExistsResponse.Object);
                
                _client.Setup(c => c.CreateIndexAsync(It.IsAny<IndexName>(), It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>(), It.IsAny<CancellationToken>()))
                    .Callback<IndexName, Func<CreateIndexDescriptor, ICreateIndexRequest>, CancellationToken>((s, i, c) => i(_indexDescriptor))
                    .ReturnsAsync(_createIndexReponse.Object);

                _indexMapper = new StubIndexMapper();
            }

            protected override async Task When()
            {
                await _indexMapper.EnureIndexExists(_client.Object);
            }

            [Test]
            public void Then_should_infer_index_name_for_type()
            {
                Assert.That(_defaultIndices.TryGetValue(typeof(StubActivity), out var indexName), Is.True);
                Assert.That(indexName, Is.EqualTo(StubIndexName));
            }

            [Test]
            public void Then_should_create_activity_index()
            {
                _client.Verify(c => c.CreateIndexAsync(StubIndexName, It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [Test]
            public void Then_should_map_index()
            {
                Assert.That(_indexMapper.MapCallCount, Is.EqualTo(1));
            }
        }

        public class When_ensuring_existant_index_exists : TestAsync
        {
            private StubIndexMapper _indexMapper;
            private readonly Mock<IElasticClient> _client = new Mock<IElasticClient>();
            private readonly Mock<IExistsResponse> _indexExistsResponse = new Mock<IExistsResponse>();
            private readonly Mock<IConnectionSettingsValues> _connectionSettings = new Mock<IConnectionSettingsValues>();
            private readonly FluentDictionary<Type, string> _defaultIndices = new FluentDictionary<Type, string>();

            protected override void Given()
            {
                _connectionSettings.Setup(s => s.DefaultIndices).Returns(_defaultIndices);

                _indexExistsResponse.Setup(i => i.Exists).Returns(true);

                _client.Setup(c => c.ConnectionSettings).Returns(_connectionSettings.Object);

                _client.Setup(c => c.IndexExistsAsync(StubIndexName, It.IsAny<Func<IndexExistsDescriptor, IIndexExistsRequest>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_indexExistsResponse.Object);

                _indexMapper = new StubIndexMapper();
            }

            protected override async Task When()
            {
                await _indexMapper.EnureIndexExists(_client.Object);
            }

            [Test]
            public void Then_should_infer_index_name_for_type()
            {
                Assert.That(_defaultIndices.TryGetValue(typeof(StubActivity), out var indexName), Is.True);
                Assert.That(indexName, Is.EqualTo(StubIndexName));
            }

            [Test]
            public void Then_should_not_create_activity_index()
            {
                _client.Verify(c => c.CreateIndexAsync(StubIndexName, It.IsAny<Func<CreateIndexDescriptor, ICreateIndexRequest>>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Test]
            public void Then_should_not_map_index()
            {
                Assert.That(_indexMapper.MapCallCount, Is.EqualTo(0));
            }
        }

        private class StubIndexMapper : IndexMapper<StubActivity>
        {
            public int MapCallCount { get; private set; }

            protected override string IndexName => StubIndexName;

            protected override void Map(TypeMappingDescriptor<StubActivity> mapper)
            {
                MapCallCount++;
            }
        }

        private class StubActivity
        {
        }
    }
}