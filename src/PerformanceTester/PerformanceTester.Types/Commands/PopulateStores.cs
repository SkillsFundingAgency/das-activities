using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Commands
{
    public class PopulateStores : ICommand, IStoreCommand
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IActivityFactory _activityFactory;
        private readonly IConfigProvider _configProvider;
        private Activity[] _activities;

        public PopulateStores(IStoreRepository storeRepository, IActivityFactory activityFactory, IConfigProvider configProvider)
        {
            _storeRepository = storeRepository;
            _activityFactory = activityFactory;
            _configProvider = configProvider;
        }

        public Task<RunDetails> DoAsync(CancellationToken cancellationToken)
        {
            _activities = _activityFactory.CreateActivities(_configProvider.Get<PopulateActivitiesParameters>()).ToArray();
            return _storeRepository.RunForEachEnabledStore(this, cancellationToken);
        }

        public Task<GroupOperationCost> DoAsync(IStore store, CancellationToken cancellationToken)
        {
            List<Task> taskList = new List<Task>();
            GroupOperationCost cost = new GroupOperationCost("Save activity");
            foreach (var activity in _activities)
            {
                var task = store
                    .PersistActivityAsync(activity, cancellationToken)
                    .ContinueWith(t => cost.StepCosts.Add(t.Result), cancellationToken);

                taskList.Add(task);
            }

            return Task.WhenAll(taskList).ContinueWith(t => cost, cancellationToken);
        }
    }
}
