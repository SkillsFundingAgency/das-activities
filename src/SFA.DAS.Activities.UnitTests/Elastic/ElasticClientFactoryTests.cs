using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Elastic;

namespace SFA.DAS.Activities.UnitTests.Elastic
{
    public static class ElasticClientFactoryTests
    {
        public class When_getting_client : Test
        {
            private IElasticClient _client;
            private IElasticClientFactory _factory;

            private readonly ActivitiesElasticConfiguration _configuration = new ActivitiesElasticConfiguration
            {
                BaseUrl = "http://localhost:9200"
            };

            private IEnumerable<Mock<IIndexMapper>> _mappers;

            protected override void Given()
            {
                _mappers = new List<Mock<IIndexMapper>>
                {
                    new Mock<IIndexMapper>(),
                    new Mock<IIndexMapper>(),
                    new Mock<IIndexMapper>()
                };

                _factory = new ElasticClientFactory(_configuration, _mappers.Select(m => m.Object));
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_correct_settings()
            {
                Assert.That(_client, Is.Not.Null);
                Assert.That(_client.ConnectionSettings.ThrowExceptions, Is.True);
            }

            [Test]
            public void Then_should_ensure_indices_exist()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExists(_client), Times.Once);
                }
            }
        }

        public class When_getting_client_multiple_times : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client1;
            private IElasticClient _client2;

            private readonly ActivitiesElasticConfiguration _configuration = new ActivitiesElasticConfiguration
            {
                BaseUrl = "http://localhost:9200"
            };

            private IEnumerable<Mock<IIndexMapper>> _mappers;

            protected override void Given()
            {
                _mappers = new List<Mock<IIndexMapper>>
                {
                    new Mock<IIndexMapper>(),
                    new Mock<IIndexMapper>(),
                    new Mock<IIndexMapper>()
                };

                _factory = new ElasticClientFactory(_configuration, _mappers.Select(m => m.Object));
            }

            protected override void When()
            {
                _client1 = _factory.GetClient();
                _client2 = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_same_client()
            {
                Assert.That(_client1, Is.SameAs(_client2));
            }

            [Test]
            public void Then_should_ensure_indices_exist_once()
            {
                foreach (var mapper in _mappers)
                {
                    mapper.Verify(m => m.EnureIndexExists(_client1), Times.Once);
                }
            }
        }

        public class When_getting_client_with_authenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            private readonly ActivitiesElasticConfiguration _configuration = new ActivitiesElasticConfiguration
            {
                BaseUrl = "http://localhost:9200",
                UserName = "elastic",
                Password = "changeme"
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_configuration, new List<IIndexMapper>());
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_enabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Username, Is.EqualTo(_configuration.UserName));
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials.Password, Is.EqualTo(_configuration.Password));
            }
        }

        public class When_getting_client_with_unauthenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;

            private readonly ActivitiesElasticConfiguration _configuration = new ActivitiesElasticConfiguration
            {
                BaseUrl = "http://localhost:9200",
                UserName = "",
                Password = ""
            };

            protected override void Given()
            {
                _factory = new ElasticClientFactory(_configuration, new List<IIndexMapper>());
            }

            protected override void When()
            {
                _client = _factory.GetClient();
            }

            [Test]
            public void Then_should_return_client_with_basic_auth_disabled()
            {
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
                Assert.That(_client.ConnectionSettings.BasicAuthenticationCredentials, Is.Null);
            }
        }
    }
}