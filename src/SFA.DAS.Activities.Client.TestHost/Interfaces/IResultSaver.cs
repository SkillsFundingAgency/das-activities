using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client.TestHost.Interfaces
{
    public interface IResultSaver
    {
        Task Save<TResult>(TResult results);
    }
}