using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using PerformanceTester.ElasticDb.Interfaces;
using PerformanceTester.Types;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;

namespace PerformanceTester.ElasticDb
{
    public class ElasticDbStore : IStore
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private IElasticClient _client;

        public string Name => "Elastic";

        public ElasticDbStore(IElasticClientFactory elasticClientFactory)
        {
            _elasticClientFactory = elasticClientFactory;
        }

        public async Task<IOperationCost> GetActivitiesForAccountAsync(long accountId)
        {
            var sw = new Stopwatch();
            sw.Start();

            var response = await _client.SearchAsync<Activity>(s => s
                .Query(q =>
                {
                    var where = q
                        .Term(t => t
                            .Field(a => a.AccountId)
                            .Value(accountId)
                        );

                    return where;
                })
                .Sort(srt => srt
                    .Descending(a => a.At)
                )
            );

            sw.Stop();

            return new OperationCost($"Fetch activities for account {accountId}", 0, sw.ElapsedTicks);
        }

        public async Task<IOperationCost> GetLatestActivitiesAsync(long accountId)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneYearAgo = today.AddYears(-1);

            var sw = new Stopwatch();
            sw.Start();

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
                        .Order(new TermsOrder { Key = "maxAt", Order = SortOrder.Descending })
                        .Size(4)
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

            sw.Stop();

            return new OperationCost($"Aggregate query for account {accountId}", 0, sw.ElapsedTicks);
        }

        public Task Initialise()
        {
            _client = _elasticClientFactory.CreateClient();
            return Task.FromResult(_client);
        }

        public async Task<IOperationCost> PersistActivityAsync(Activity activity, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();
            var indexResponse = await _client.IndexAsync(activity, cancellationToken: cancellationToken);
            sw.Stop();

            return new OperationCost("Upsert activity", -1, sw.ElapsedTicks);
        }
    }
}
