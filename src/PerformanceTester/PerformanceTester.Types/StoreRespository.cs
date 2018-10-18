using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;

namespace PerformanceTester.Types
{
    public class StoreRespository : IStoreRepository
    {
        private readonly ConcurrentDictionary<IStore, Task> _initialisationTasks = new ConcurrentDictionary<IStore, Task>();

        private readonly Dictionary<string, IStore> _availableStores = new Dictionary<string, IStore>(StringComparer.InvariantCultureIgnoreCase);

        private readonly List<IStore> _enabledStores = new List<IStore>();

        public StoreRespository(IStore[] availableStores)
        {
            foreach (var availableStore in availableStores)
            {
                _availableStores.Add(availableStore.Name, availableStore);
            }
        }

        public IReadOnlyList<IStore> EnabledStores => _enabledStores;

        public IEnumerable<IStore> AllStores => _availableStores.Values; 

        public void EnableStore(string name)
        {
            var store = GetAvailableStoreByName(name);
            EnableStore(store);
        }

        public bool IsStoreEnabled(string name)
        {
            var store = GetAvailableStoreByName(name);
            return _enabledStores.Contains(store);
        }

        public void DisableStore(string name)
        {
            var store = GetAvailableStoreByName(name);

            _enabledStores.Remove(store);
        }

        public void EnableAllStores()
        {
            foreach (var store in _availableStores.Values)
            {
                EnableStore(store);
            }
        }

        private void EnableStore(IStore store)
        {
            if (!_enabledStores.Contains(store))
            {
                _enabledStores.Add(store);
            }
        }

        private IStore GetAvailableStoreByName(string name)
        {
            if (!_availableStores.TryGetValue(name, out IStore store))
            {
                throw new UnknownStoreNameException(name, _availableStores.Keys);
            }

            return store;
        }

        public Task<RunDetails> RunForEachEnabledStore(IStoreCommand command, CancellationToken cancellationToken)
        {
            RunDetails result = new RunDetails();

            foreach (IStore enabledStore in EnabledStores)
            {
                EnsureStoreInitialised(enabledStore).Wait(cancellationToken);

                var storeTaskDetails = Run(result, command, enabledStore, cancellationToken);
                result.StoreDetails.Add(storeTaskDetails);
            }

            return Task.WhenAll(result.Tasks).ContinueWith<RunDetails>((Func<Task, RunDetails>)(t => result), cancellationToken);
        }

        public StoreTaskDetails Run(RunDetails runDetails, IStoreCommand command, IStore store, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Task<GroupOperationCost> task = command.DoAsync(store, cancellationToken);
            StoreTaskDetails taskDetails = new StoreTaskDetails(command, store, task);
            task.ContinueWith(t =>
            {
                sw.Stop();
                taskDetails.Elapsed = sw.Elapsed;
                if (task.IsCompleted)
                {
                    taskDetails.Cost = task.Result;
                }
            }, cancellationToken);

            return taskDetails;
        }

        private Task EnsureStoreInitialised(IStore store)
        {
            return _initialisationTasks.GetOrAdd(store, s =>
            {
                Console.WriteLine($"Initialising store {store.Name}");
                return s.Initialise();
            });
        } 
    }
}