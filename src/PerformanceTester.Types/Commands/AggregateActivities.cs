using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Parameters;
using PerformanceTester.Types.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Commands
{
    public class AggregateActivities : ICommand, IStoreCommand
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IConfigProvider _configProvider;
        private AggregateActivitiesParameters _config;

        public AggregateActivities(IStoreRepository storeRepository, IConfigProvider configProvider)
        {
            _storeRepository = storeRepository;
            _configProvider = configProvider;
        }

        public Task<RunDetails> DoAsync(CancellationToken cancellationToken)
        {
            _config = _configProvider.Get<AggregateActivitiesParameters>();
            return _storeRepository.RunForEachEnabledStore(this, cancellationToken);
        }

        public Task<GroupOperationCost> DoAsync(IStore store, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var cost = new GroupOperationCost("Aggregate Activities");

                foreach(var accountId in NumberRange.ToInts(_config.AccountIds))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    var operationCost = await store.GetLatestActivitiesAsync(accountId);
                    cost.StepCosts.Add(operationCost);
                }

                return cost;

            }, cancellationToken);
        }
    }
}
