using System.Threading.Tasks;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IAzureBlobRepository
    {
        Task SerialiseObjectToLog<T>(string blobname, T instance);
    }
}