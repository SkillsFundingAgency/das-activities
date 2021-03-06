﻿using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.Types;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;

namespace PerformanceTester.MSSqlDb
{

    public class MsSqlDbStore : IStore
    {
        private readonly IDbContextFactory _dbContextFactory;

        public string Name => "MsSql";

        public MsSqlDbStore(IDbContextFactory elasticClientFactory)
        {
            _dbContextFactory = elasticClientFactory;
        }

        public async Task<IOperationCost> GetActivitiesForAccountAsync(long accountId)
        {

            using (var db = _dbContextFactory.Create())
            {
                var sw = new Stopwatch();
                sw.Start();

                var response = await db.Activities
                    .Where(activity => activity.AccountId == accountId)
                    .OrderByDescending(activity => activity.At)
                    .ToListAsync();

                sw.Stop();

                return new OperationCost($"Fetch activities for account {accountId}", 0, sw.ElapsedTicks);
            }
        }

        public async Task<IOperationCost> GetLatestActivitiesAsync(long accountId)
        {
            using (var db = _dbContextFactory.Create())
            {
                var sw = new Stopwatch();
                sw.Start();

                var response = await db.Database.ExecuteSqlCommandAsync(@"
                                        SELECT	Id, AccountId, At, Created, Description, Type 
                                        FROM	(
	                                        SELECT 
		                                        ROW_NUMBER() OVER(PARTITION BY A.Type ORDER BY A.At DESC) as rn,
		                                        A.[Id] AS [Id], 
		                                        A.[AccountId] AS [AccountId], 
		                                        A.[At] AS [At], 
		                                        A.[Created] AS [Created], 
		                                        A.[Description] AS [Description], 
		                                        A.[Type] AS [Type]
		                                        FROM [dbo].[Activities] AS A
	                                        WHERE A.AccountId = 5 
		                                          AND A.At BETWEEN DATEADD(YY, -2, GetUtcDate()) AND GetUtcDate()
	                                        ) AS T1
                                        WHERE T1.rn <= 5;
                                        ");
                sw.Stop();

                return new OperationCost($"Aggregate query for account {accountId}", 0, sw.ElapsedTicks);
            }
        }

        public Task Initialise()
        {
            return Task.Run(() =>
            {
                using (_dbContextFactory.Create())
                {
                }
            });
        }

        public async Task<IOperationCost> PersistActivityAsync(Activity activity, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();
            using (var db = _dbContextFactory.Create())
            {
                db.Activities.Add(activity);
                await db.SaveChangesAsync(cancellationToken);
            }
            sw.Stop();

            return new OperationCost("Upsert activity", -1, sw.ElapsedTicks);
        }
    }
}
