using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Activities.Domain.Configurations;
using SFA.DAS.Activities.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using Nest;
using NuGet;

namespace SFA.DAS.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly ElasticClient _elasticClient;
        private readonly ILog _logger;

        public ActivitiesRepository(ActivitiesConfiguration configuration, ILog logger)
        {
            //var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            _elasticClient =new ElasticClient(elasticSettings);

            _logger = logger;
        }

        public async Task<IEnumerable<Activity>> GetActivities(string accountId, string type)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.OwnerId)
                        .Query(accountId)
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.ActivityType)
                        .Query(type)
                    )
                )
            );

            return searchResponse.Documents;
        }

        public async Task<IEnumerable<Activity>> GetActivities(string ownerId)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.OwnerId)
                        .Query(ownerId)
                    )
                )
            );

            return searchResponse.Documents;
        }

        public async Task<Activity> GetActivity(Activity activity)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.OwnerId)
                        .Query(activity.OwnerId)
                    )
                )
                .Query(q=>q
                    .Match(m=>m
                    .Field(f=>f.ActivityType)
                    .Query(activity.ActivityType.ToString())
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Description)
                        .Query(activity.Description)
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Url)
                        .Query(activity.Url)
                    )
                )
            );

            return searchResponse.Documents.FirstOrDefault();
        }

        public async Task SaveActivity(Activity activity)
        {
            var activityAlreadyExists = await GetActivity(activity);

            if (activityAlreadyExists ==null)
                await (_elasticClient.IndexAsync(activity));
        }
    }
}
