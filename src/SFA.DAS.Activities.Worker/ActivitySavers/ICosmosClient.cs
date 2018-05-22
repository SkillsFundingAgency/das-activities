using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.Activities.Worker.ActivitySavers
{
    public interface ICosmosClient
    {
        DocumentClient Client { get; }
        Task UpsertDocumentAsync(string collectionName, object entity);
    }
}