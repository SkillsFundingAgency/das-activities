
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Activities.Worker.Providers
{
    public interface IActivityDataProcessor
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
