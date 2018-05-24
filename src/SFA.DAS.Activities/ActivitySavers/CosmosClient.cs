using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.ActivitySavers
{
    class CosmosClient : ICosmosClient
    {
        private readonly IMessageServiceBusConfiguration _config;
        private readonly Lazy<DocumentClient> _client;
        private readonly ConcurrentDictionary<string, Uri> _collections = new ConcurrentDictionary<string, Uri>();
        private RequestOptions _requestOptions;

        public CosmosClient(IMessageServiceBusConfiguration messageServiceBusConfiguration)
        {
            _client = new Lazy<DocumentClient>(InitialiseClient);
            _config = messageServiceBusConfiguration;
        }

        public DocumentClient Client => _client.Value;

        public Task UpsertDocumentAsync(string collectionName, object entity)
        {
            var collectionUri = _collections.GetOrAdd(collectionName, GetCollectionUri);

            return Client.UpsertDocumentAsync(collectionUri, entity, _requestOptions);
        }

        private Uri GetCollectionUri(string collectionName)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(_config.CosmosDatabase, collectionName);

            var databaseUri = UriFactory.CreateDatabaseUri(_config.CosmosDatabase);
            RunWithTimeOut(Client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, new DocumentCollection { Id = collectionName }));
            return collectionUri;
        }

        private DocumentClient InitialiseClient()
        {
            var client = new DocumentClient(new Uri(_config.CosmosEndpointUrl), _config.CosmosPrimaryKey);
            _requestOptions = new RequestOptions
            {
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            RunWithTimeOut(client.CreateDatabaseIfNotExistsAsync(new Database { Id = _config.CosmosDatabase }));
            return client;
        }

        private static readonly TimeSpan DefaultTimeOut = new TimeSpan(0, 0, 30);

        private void RunWithTimeOut(Task task)
        {
            RunWithTimeOut(task, DefaultTimeOut);
        }

        private void RunWithTimeOut(Task task, TimeSpan timeout)
        {
            task.Wait(timeout);

            if (!task.IsCompleted)
            {
                throw new Exception("Timed out waiting for operation to complete");
            }
        }
    }
}