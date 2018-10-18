using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client.TestHost.Interfaces
{
    public interface ICommand
    {
        Task DoAsync(CancellationToken cancellationToken);
    }
}