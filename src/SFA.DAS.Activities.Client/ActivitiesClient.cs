using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Client.Elastic;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClient : IActivitiesClient
    {
        private const string IndexName = "activities";

        private readonly IIndexAutoMapper _indexAutoMapper;
        private readonly IElasticClient _client;

        public ActivitiesClient(IIndexAutoMapper indexAutoMapper, IElasticClient client)
        {
            _indexAutoMapper = indexAutoMapper;
            _client = client;
        }

        public async Task<ActivitiesResult> GetActivities(long accountId, int? take = null, DateTime? from = null, DateTime? to = null)
        {
            await _indexAutoMapper.EnureIndexExists<Activity>(IndexName);

            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneYearAgo = today.AddYears(-1);

            if (take == null)
            {
                take = 50;
            }

            if (from == null)
            {
                from = oneYearAgo;
            }

            if (to == null)
            {
                to = now;
            }

            var response = await _client.SearchAsync<Activity>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.AccountId)
                        .Value(accountId)
                    ) && q
                    .DateRange(r => r
                        .Field(f => f.At)
                        .GreaterThanOrEquals(DateMath.Anchored(from.Value).RoundTo(TimeUnit.Day))
                        .LessThanOrEquals(DateMath.Anchored(to.Value).RoundTo(TimeUnit.Day))
                    )
                )
                .Sort(srt => srt
                    .Descending(a => a.At)
                    .Descending("_uid")
                )
                .Take(take.Value)
            );

            return new ActivitiesResult
            {
                Activities = response.Documents,
                Total = response.Total
            };
        }

        public async Task<AggregatedActivitiesResult> GetLatestActivities(long accountId)
        {
            await _indexAutoMapper.EnureIndexExists<Activity>(IndexName);

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

            var aggregates = (
                from type in response.Aggs.Terms("activities_per_type").Buckets
                let date = type.DateHistogram("activities_per_day").Buckets.FirstOrDefault()
                let topHit = date?.TopHits("top_activity_hit").Hits<Activity>().Select(h => h.Source).First()
                where topHit != null
                select new AggregatedActivityResult
                {
                    TopHit = topHit,
                    Count = date.DocCount.Value
                })
                .ToList();

            return new AggregatedActivitiesResult
            {
                Aggregates = aggregates,
                Total = response.Total
            };
        }
    }
}