using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using SFA.DAS.NLog.Logger;
using Nest;
using NuGet;
using SFA.DAS.Activities.Application;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Application.Repositories;

namespace SFA.DAS.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly IActivitiesConfiguration _configuration;
        private readonly ILog _logger;

        private string _currentIndex;
        private ElasticClient _elasticClient;

        public ActivitiesRepository(IActivitiesConfiguration configuration, ILog logger)
        {
           _configuration = configuration;
           _logger = logger;
        }
        public async Task SaveActivity(Activity activity)
        {
            await ElasticClient.IndexAsync(activity);
        }

        private string GetIndexName()
        {
            return string.Format(_configuration.ElasticSearchIndexFormat, _configuration.EnvironmentName,
                DateTime.UtcNow.ToString("yyyy-MM-dd"));
        }

        private ElasticClient ElasticClient
        {
            get
            {
                var indexName = GetIndexName();
                if (string.IsNullOrEmpty(_currentIndex) || indexName != _currentIndex)
                {
                    var connectionSettings = new ConnectionSettings(new Uri(_configuration.ElasticServerBaseUrl))
                        .DefaultIndex(indexName);
                    _currentIndex = indexName;
                    if (_configuration.RequiresAuthentication)
                    {
                        connectionSettings.BasicAuthentication(_configuration.ElasticSearchUserName,
                            _configuration.ElasticSearchPassword);
                    }
                    _elasticClient = new ElasticClient(connectionSettings);

                    if (!IndexExists(indexName))
                    {
                        CreateIndex(indexName);
                    }
                }

                return _elasticClient;
            }
        }

        private bool IndexExists(string name)
        {
            var request = new IndexExistsRequest(name);
            var response = _elasticClient.IndexExists(request);
            if (!response.IsValid)
            {
                throw response.OriginalException;
            }

            return response.Exists;
        }

        private void CreateIndex(string name)
        {
            var request = new CreateIndexRequest(name);
            var response = _elasticClient.CreateIndex(request);
            if (!response.IsValid)
            {
                throw response.OriginalException;
            }
        }
    }
}
