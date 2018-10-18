using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClient : IActivitiesClient
    {
        private readonly IElasticClient _client;

        public ActivitiesClient(IElasticClient client)
        {
            _client = client;
        }

        public async Task<ActivitiesResult> GetActivities(ActivitiesQuery query)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneYearAgo = today.AddYears(-1);
            var take = query.Take ?? 50;
            var from = query.From ?? oneYearAgo;
            var to = query.To ?? now;

            if (take > 500)
            {
                take = 500;
            }

            var response = await _client.SearchAsync<Activity>(s => s
                .Query(q =>
                {
                    var where = q
                        .Term(t => t
                            .Field(a => a.AccountId)
                            .Value(query.AccountId)
                        ) && q
                        .DateRange(r => r
                            .Field(a => a.At)
                            .GreaterThanOrEquals(DateMath.Anchored(from).RoundTo(TimeUnit.Day))
                            .LessThanOrEquals(DateMath.Anchored(to).RoundTo(TimeUnit.Day))
                        );

                    if (query.Category != null)
                    {
                        where &= q
                            .Terms(t => t
                                .Field(a => a.Type)
                                .Terms(query.Category.Value.GetActivityTypes())
                            );
                    }

                    if (query.Data != null && query.Data.Any())
                    {
                        where &= q.Nested(n => n
                            .Path(p => p.Data)
                            .Query(nq =>
                            {
                                QueryContainer nestedWhere = null;

                                foreach (var filter in query.Data)
                                {
                                    nestedWhere &= nq
                                        .Term(t => t
                                            .Field(a => a.Data[filter.Key + ".keyword"])
                                            .Value(filter.Value)
                                        );
                                }

                                return nestedWhere;
                            })
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(query.Term))
                    {
                        where &= q
                            .MultiMatch(m => m
                                .Type(TextQueryType.CrossFields)
                                .Fields(f => f
                                    .Field(a => a.Description)
                                    .Field(a => a.Data["*"])
                                )
                                .Query(query.Term)
                                .Operator(Operator.And)
                            );
                    }

                    return where;
                })
                .Sort(srt => srt
                    .Descending(a => a.At)
                    .Descending("_uid")
                )
                .Take(take)
            );

            return new ActivitiesResult
            {
                Activities = response.Documents,
                Total = response.Total
            };
        }

        public async Task<AggregatedActivitiesResult> GetLatestActivities(long accountId)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneYearAgo = today.AddYears(-1);

            var response = await _client.SearchAsync<Activity>(s => s
                .Query(q => q
                    .Term(t => t
                        .Field(a => a.AccountId)
                        .Value(accountId)
                    ) && q
                    .DateRange(r => r
                        .Field(a => a.At)
                        .GreaterThanOrEquals(DateMath.Anchored(oneYearAgo).RoundTo(TimeUnit.Day))
                        .LessThanOrEquals(DateMath.Anchored(now).RoundTo(TimeUnit.Day))
                    )
                )
                .Size(0)
                .Aggregations(aggs => aggs
                    .Terms("activitiesByType", t => t
                        .Field(a => a.Type)
                        .Order(new TermsOrder{Key = "maxAt", Order = SortOrder.Descending})
                        .Aggregations(aggs2 => aggs2
                            .Max("maxAt", m => m
                                .Field(a => a.At)
                            )
                            .DateHistogram("activitiesByDay", d => d
                                .Field(a => a.At)
                                .Interval(DateInterval.Day)
                                .MinimumDocumentCount(1)
                                .ExtendedBoundsDateMath(oneYearAgo, now)
                                .Order(HistogramOrder.KeyDescending)
                                .Aggregations(aggs3 => aggs3
                                    .TopHits("activityTopHit", th => th
                                        .Sort(srt => srt
                                            .Descending(a => a.At)
                                            .Descending("_uid")
                                        )
                                        .Size(1)
                                    )
                                )
                            )
                        )
                    )
                )
            );

            var aggregates = response.Aggs.Terms("activitiesByType").Buckets
                .SelectMany(at => at.DateHistogram("activitiesByDay").Buckets)
                .Select(
                    activityTypeOnDay =>
                    {
                        return new AggregatedActivityResult
                        {
                            TopHit = activityTypeOnDay.TopHits("activityTopHit").Hits<Activity>().Select(h => h.Source).First(),
                            Count = activityTypeOnDay.DocCount.Value
                        };
                    })
                .OrderByDescending(atod => atod.TopHit.Created)
                .Take(4)
                .ToList();

            return new AggregatedActivitiesResult
            {
                Aggregates = aggregates,
                Total = response.Total
            };
        }
    }
}