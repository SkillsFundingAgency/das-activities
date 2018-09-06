using System.Collections.ObjectModel;
using Microsoft.Azure.Documents;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public class DocumentCollectionConfigurator : IDocumentCollectionConfigurator
    {
        private readonly ICosmosConfiguration _configuration;

        public DocumentCollectionConfigurator(ICosmosConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DocumentCollection ConfigureCollection(string collection)
        {
            if (collection == _configuration.CosmosCollectionName)
            {
                return BuildActivitiesCollection(collection);
            }

            return BuildDefaultCollection(collection);
        }

        private DocumentCollection BuildActivitiesCollection(string collectionName)
        {
            var collection = new DocumentCollection { Id = collectionName };

            collection.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath
                {
                    Path = "/messageId/?",
                    Indexes = new Collection<Index> {
                        new RangeIndex(DataType.String) { Precision = 36 } }
                });

            // Default for everything else
            collection.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath
                {
                    Path = "/*",
                    Indexes = new Collection<Index> {
                        new HashIndex(DataType.String) { Precision = 3 },
                        new RangeIndex(DataType.Number) { Precision = -1 }
                    }
                });

            return collection;
        }

        private DocumentCollection BuildDefaultCollection(string collectionName)
        {
            var collection = new DocumentCollection { Id = collectionName };

            collection.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath
                {
                    Path = "/*",
                    Indexes = new Collection<Index> {
                        new HashIndex(DataType.String) { Precision = 3 },
                        new RangeIndex(DataType.Number) { Precision = -1 }
                    }
                });

            return collection;
        }

    }
}