using PerformanceTester.Types.Types;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Interfaces
{
    public interface IStore
    {
        string Name { get; }

        Task Initialise();

        Task<IOperationCost> PersistActivityAsync(Activity activity, CancellationToken cancellationToken);

        Task<IOperationCost> GetActivitiesForAccountAsync(long accoundId);

    }
}
