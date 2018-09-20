using PerformanceTester.Types.Types;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Interfaces
{
    public interface IStoreCommand
    {
        Task<GroupOperationCost> DoAsync(IStore store, CancellationToken cancellationToken);
    }
}