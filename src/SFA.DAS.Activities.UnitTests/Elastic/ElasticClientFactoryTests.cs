﻿using Nest;
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

            protected override void Given()
            {
                _factory = new ElasticClientFactory(new ActivitiesElasticConfiguration { BaseUrl = "http://localhost:9200" });
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
        }

        public class When_getting_client_with_authenticated_connection : Test
        {
            private IElasticClientFactory _factory;
            private IElasticClient _client;
            private ActivitiesElasticConfiguration _configuration;

            protected override void Given()
            {
                _configuration = new ActivitiesElasticConfiguration
                {
                    BaseUrl = "http://localhost:9200",
                    UserName = "elastic",
                    Password = "changeme"
                };

                _factory = new ElasticClientFactory(_configuration);
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
            private ActivitiesElasticConfiguration _configuration;

            protected override void Given()
            {
                _configuration = new ActivitiesElasticConfiguration
                {
                    BaseUrl = "http://localhost:9200",
                    UserName = "",
                    Password = ""
                };

                _factory = new ElasticClientFactory(_configuration);
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