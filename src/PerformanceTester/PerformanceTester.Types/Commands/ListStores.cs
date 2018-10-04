using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Parameters;
using PerformanceTester.Types.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Commands
{
    public class ListStores : ICommand
    {
        private readonly IStoreRepository _storeRepository;

        public ListStores(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public Task<RunDetails> DoAsync(CancellationToken cancellationToken)
        {
            foreach (var store in _storeRepository.AllStores.OrderBy(s => s.Name))
            {
                Console.WriteLine($"{store.Name, 0-30} {store.GetType().FullName}");
            }

            return Task.FromResult(new RunDetails());
        }
    }
}
