using Microsoft.Azure.Documents;

namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IDocumentCollectionConfigurator
    {
        DocumentCollection ConfigureCollection(string collection);
    }
}