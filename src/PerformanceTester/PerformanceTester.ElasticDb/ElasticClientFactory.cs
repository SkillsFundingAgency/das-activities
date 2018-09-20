using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using PerformanceTester.ElasticDb.Interfaces;
using PerformanceTester.Types;

namespace PerformanceTester.ElasticDb
{
    public class ElasticClientFactory : IElasticClientFactory, IDisposable
    {
        private const string IndexName = "activities";

        private readonly Lazy<IConnectionSettingsValues> _connectionSettings;
        private readonly IConfigProvider _configProvider;
        private readonly IEnumerable<IIndexMapper> _indexMappers;

        public ElasticClientFactory(IConfigProvider configProvider, IEnumerable<IIndexMapper> indexMappers)
        {
            _connectionSettings = new Lazy<IConnectionSettingsValues>(InitialiseConnectionSettings);
            _configProvider = configProvider;
            _indexMappers = indexMappers;
        }

        public IElasticClient CreateClient()
        {
            var client = new ElasticClient(_connectionSettings.Value);
            EnsureIndexInitialised(client).Wait();
            return client;
        }

        public void Dispose()
        {
            if (_connectionSettings.IsValueCreated)
            {
                _connectionSettings.Value.Dispose();
            }
        }

        private Task _initialiseTask = null;

        private Task EnsureIndexInitialised(IElasticClient client)
        {
            return _initialiseTask ?? (_initialiseTask =
                       Task.WhenAll(_indexMappers.Select(m =>
                       {
                           var config = _configProvider.Get<ElasticConfig>();
                           return m.EnureIndexExistsAsync(config.Environment, client);

                       }).ToArray()));
        }

        private IConnectionSettingsValues InitialiseConnectionSettings()
        {
            var config = _configProvider.Get<ElasticConfig>();

            var connectionSettings = new Nest
                    .ConnectionSettings(new SingleNodeConnectionPool(new Uri(config.Url)))
                    .ThrowExceptions();

            connectionSettings.BasicAuthentication(config.UserName, config.Password);

            return connectionSettings;
        }
    }
}