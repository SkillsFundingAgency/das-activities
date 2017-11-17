using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ElasticClient _elasticClient;
        private readonly ILog _logger;

        public ActivitiesRepository(ActivitiesConfiguration configuration, ILog logger)
        {
            var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl)).DefaultIndex("activities");
            _elasticClient =new ElasticClient(elasticSettings);

            _logger = logger;
        }

        public Task<IEnumerable<Activity>> GetActivities(long accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Activity> GetActivity(Activity activity)
        {
            var searchResponse = await _elasticClient.SearchAsync<Activity>(s => s
                .From(0)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.AccountId)
                        .Query(activity.AccountId.ToString())
                    )
                )
                .Query(q=>q
                    .Match(m=>m
                    .Field(f=>f.TypeOfActivity)
                    .Query(activity.TypeOfActivity.ToString())
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
