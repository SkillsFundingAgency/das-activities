using System;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Models;

namespace SFA.DAS.Activities.Data
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly ElasticConfiguration _elasticConfig;
        private readonly EnvironmentConfiguration _envConfig;

        private string _currentIndex;
        private ElasticClient _elasticClient;

        public ActivitiesRepository(ElasticConfiguration elasticConfig, EnvironmentConfiguration envConfig)
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
            return string.Format(_elasticConfig.IndexFormat, _envConfig.Name, DateTime.UtcNow.ToString("yyyy-MM-dd"));
        }

        private ElasticClient ElasticClient
        {
            get
            {
                var indexName = GetIndexName();
                if (string.IsNullOrEmpty(_currentIndex) || indexName != _currentIndex)
                {
                    var connectionSettings = new ConnectionSettings(new Uri(_elasticConfig.BaseUrl))
                        .DefaultIndex(indexName);
                    _currentIndex = indexName;
                    if (_elasticConfig.RequiresAuthentication)
                    {
                        connectionSettings.BasicAuthentication(_elasticConfig.UserName,
                            _elasticConfig.Password);
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
