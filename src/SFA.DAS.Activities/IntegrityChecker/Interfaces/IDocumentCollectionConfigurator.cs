using Microsoft.Azure.Documents;

namespace SFA.DAS.Activities.ActivitySavers
{
    public interface IDocumentCollectionConfigurator
    {
        DocumentCollection ConfigureCollection(string collection);
    }
}