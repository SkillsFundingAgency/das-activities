using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Client.Extensions;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesService : IActivitiesService
    {
        private const string IndexName = "activities";

        private readonly IElasticClient _client;
        private bool _ensuredIndexExists;

        public ActivitiesService(IElasticClient client)
        {
            _client = client;
        }

        public async Task AddActivity(Activity activity)
        {
            await EnureIndexExists();
            await _client.IndexAsync(activity, i => i.Index(IndexName));
        }

        public async Task<IEnumerable<AggregatedActivity>> GetAggregatedActivities(long accountId)
        {
            await EnureIndexExists();

            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneYearAgo = today.AddYears(-1);

            var response = await _client.SearchAsync<Activity>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.AccountId)
                        .Value(accountId)
                    )
                )
                .Size(0)
                .Aggregations(a => a
                    .Terms("activities_per_type", t => t
                        .Field(f => f.Type)
                        .OrderDescending("max_at")
                        .Size(4)
                        .Aggregations(a2 => a2
                            .Max("max_at", m => m
                                .Field(f => f.At)
                            )
                            .DateHistogram("activities_per_day", d => d
                                .Field(f => f.At)
                                .Interval(DateInterval.Day)
                                .MinimumDocumentCount(1)
                                .Format("yyyy-MM-dd'T'HH:mm:ss")
                                .ExtendedBounds(oneYearAgo, now)
                                .Order(HistogramOrder.KeyDescending)
                                .Aggregations(a3 => a3
                                    .TopHits("top_activity_hit", th => th
                                        .Sort(srt => srt
                                            .Field(f => f.At)
                                            .Order(SortOrder.Descending)
                                        )
                                        .Size(1)
                                    )
                                )
                            )
                        )
                    )
                )
            );

            var activities = (
                from type in response.Aggs.Terms("activities_per_type").Buckets
                let date = type.DateHistogram("activities_per_day").Buckets.FirstOrDefault()
                let activity = date?.TopHits("top_activity_hit").Hits<Activity>().Select(h => h.Source).First()
                where activity != null
                select new AggregatedActivity
                {
                    Type = type.Key.ToEnum<ActivityType>(),
                    AccountId = activity.AccountId,
                    At = activity.At,
                    CreatorName = activity.CreatorName,
                    CreatorUserRef = activity.CreatorUserRef,
                    PayeScheme = activity.PayeScheme,
                    ProviderUkprn = activity.ProviderUkprn,
                    Count = date.DocCount.Value
                })
                .ToList();

            return activities;
        }

        private async Task EnureIndexExists()
        {
            if (!_ensuredIndexExists)
            {
                var response = await _client.IndexExistsAsync(IndexName);

                if (!response.Exists)
                {
                    await _client.CreateIndexAsync(IndexName, i => i.Mappings(ms => ms.Map<Activity>(m => m.AutoMap())));
                }

                _ensuredIndexExists = true;
            }
        }
    }
}