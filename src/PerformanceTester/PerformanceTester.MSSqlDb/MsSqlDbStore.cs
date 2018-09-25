using System.Data.Entity;
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
