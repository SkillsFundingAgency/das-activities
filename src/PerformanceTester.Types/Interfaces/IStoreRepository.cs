using PerformanceTester.Types.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Interfaces
{
    public interface IStoreRepository
    {
        IReadOnlyList<IStore> EnabledStores { get; }

        bool IsStoreEnabled(string name);

        void EnableStore(string name);

        void DisableStore(string name);

        void EnableAllStores();

        Task<RunDetails> RunForEachEnabledStore(IStoreCommand command, CancellationToken cancellationToken);
    }
}
