using System;
using Elasticsearch.Net;
using Nest;

namespace SFA.DAS.Activities.Client.Elastic
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ElasticClient _client;
        private readonly ConnectionSettings _settings;

        public ElasticClientFactory(ActivitiesClientConfiguration configuration)
        {
            _settings = new ConnectionSettings(new StaticConnectionPool(new [] { new Uri(configuration.BaseUrl) })).ThrowExceptions();

            if (configuration.RequiresAuthentication)
            {
                _settings.BasicAuthentication(configuration.UserName, configuration.Password);
            }

            _client = new ElasticClient(_settings);
        }

        public IElasticClient GetClient()
        {
            return _client;
        }

        public void Dispose()
        {
            ((IDisposable)_settings).Dispose();
        }
    }
}