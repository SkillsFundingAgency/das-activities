using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using NuGet;
using SFA.DAS.Acitivities.Core.Configuration;
using SFA.DAS.Activities.Application;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.Application.Configurations;
using SFA.DAS.Activities.Application.Repositories;

namespace SFA.DAS.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly IOptions<ElasticConfiguration> _elasticConfig;
        private readonly IOptions<EnvironmentConfiguration> _envConfig;

        private string _currentIndex;
        private ElasticClient _elasticClient;

        public ActivitiesRepository(IOptions<ElasticConfiguration> elasticConfig, IOptions<EnvironmentConfiguration> envConfig)
        {
            _elasticConfig = elasticConfig;
            _envConfig = envConfig;
        }
        public async Task SaveActivity(Activity activity)
        {
            var response = await ElasticClient.IndexAsync(activity);
            if (!response.IsValid)
            {
                throw new ApplicationException("Problem saving activity", response.ApiCall?.OriginalException);
            }
        }

        private string GetIndexName()
        {
            return string.Format(_elasticConfig.Value.IndexFormat, _envConfig.Value.Name,
                DateTime.UtcNow.ToString("yyyy-MM-dd"));
        }

        private ElasticClient ElasticClient
        {
            get
            {
                var indexName = GetIndexName();
                if (string.IsNullOrEmpty(_currentIndex) || indexName != _currentIndex)
                {
                    var connectionSettings = new ConnectionSettings(new Uri(_elasticConfig.Value.BaseUrl))
                        .DefaultIndex(indexName);
                    _currentIndex = indexName;
                    if (_elasticConfig.Value.RequiresAuthentication)
                    {
                        connectionSettings.BasicAuthentication(_elasticConfig.Value.UserName,
                            _elasticConfig.Value.Password);
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
                throw new ApplicationException("Problem checking if index exists", response.OriginalException);
            }

            return response.Exists;
        }

        private void CreateIndex(string name)
        {
            var request = new CreateIndexRequest(name);
            var response = _elasticClient.CreateIndex(request);
            if (!response.IsValid)
            {
                throw new ApplicationException("Problem creating index", response.ApiCall?.OriginalException);
            }
        }
    }
}
