using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Elastic
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ElasticClient _client;
        private readonly ConnectionSettings _settings;

        public ElasticClientFactory(IElasticConfiguration configuration, IEnumerable<IIndexMapper> indexMappers)
        {
            _settings = new ConnectionSettings(new StaticConnectionPool(new [] { new Uri(configuration.BaseUrl) })).ThrowExceptions();

            if (configuration.RequiresAuthentication)
            {
                _settings.BasicAuthentication(configuration.UserName, configuration.Password);
            }

            _client = new ElasticClient(_settings);

            Task.WaitAll(indexMappers.Select(m => m.EnureIndexExists(_client)).ToArray());
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