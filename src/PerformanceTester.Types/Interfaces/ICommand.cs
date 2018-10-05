using PerformanceTester.Types.Types;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Types.Interfaces
{
    public interface ICommand
    {
        Task<RunDetails> DoAsync(CancellationToken cancellationToken);
    }
}