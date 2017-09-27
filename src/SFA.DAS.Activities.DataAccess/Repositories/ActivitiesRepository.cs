using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Activities.Domain.Configurations;
using SFA.DAS.Activities.Domain.Models;
using SFA.DAS.Activities.Domain.Repositories;
using SFA.DAS.NLog.Logger;
using Nest;
using NuGetProject.Enums;

namespace SFA.DAS.Activities.DataAccess.Repositories
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly ElasticClient _elasticClient;

        public ActivitiesRepository(ActivitiesConfiguration configuration, ILog logger)
        {
            //var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            _elasticClient =new ElasticClient(elasticSettings);
        }

        public async Task<IEnumerable<Activity>> GetActivities(string accountId, ActivityType type)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(accountId)
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
                        .Field(f => f.AccountId)
                        .Query(activity.AccountId)
                    )
                )
                .Query(q=>q
                    .Match(m=>m
                    .Field(f=>f.Type)
                    .Query(activity.Type.ToString())
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
