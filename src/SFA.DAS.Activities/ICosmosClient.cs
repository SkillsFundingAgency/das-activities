using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.Activities
{
    public interface ICosmosClient
    {
        DocumentClient Client { get; }
        Task UpsertDocumentAsync(string collectionName, object entity);
    }
}