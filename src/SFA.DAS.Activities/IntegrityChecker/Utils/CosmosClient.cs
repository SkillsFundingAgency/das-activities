using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.ActivitySavers
{
    class CosmosClient : ICosmosClient
    {
        private readonly ICosmosConfiguration _config;
        private readonly IDocumentCollectionConfigurator _documentCollectionConfigurator;
        private readonly Lazy<DocumentClient> _client;
        private readonly ConcurrentDictionary<string, DocumentCollection> _knownCollections;
        private readonly ConcurrentDictionary<string, Uri> _collections = new ConcurrentDictionary<string, Uri>();
        private RequestOptions _requestOptions;

        public CosmosClient(ICosmosConfiguration messageServiceBusConfiguration, IDocumentCollectionConfigurator documentCollectionConfigurator)
        {
            _client = new Lazy<DocumentClient>(InitialiseClient);
            _documentCollectionConfigurator = documentCollectionConfigurator;
            _config = messageServiceBusConfiguration;
            _knownCollections = new ConcurrentDictionary<string, DocumentCollection>();
        }

        public DocumentClient Client => _client.Value;

        public Task UpsertDocumentAsync(string collection, object entity)
        {
            var collectionUri = GetCollectionUri(collection);

            return Client.UpsertDocumentAsync(collectionUri, entity, _requestOptions);
        }

        public async Task<CosmosClientQueryResult<TDocumentType>> GetPageAsync<TDocumentType, TKey>(string collection, string continuationToken, Expression<Func<TDocumentType, TKey>> orderby, int pageSize)
        {
            var collectionUri = GetCollectionUri(collection);

            var options = new FeedOptions
            {
                MaxItemCount = pageSize,
                RequestContinuation = continuationToken
            };

            var query = _client.Value
                .CreateDocumentQuery<TDocumentType>(collectionUri, options) 
                .OrderBy(orderby)
                .AsDocumentQuery();

            var page = await query.ExecuteNextAsync<TDocumentType>();


            var pageResult = new CosmosClientQueryResult<TDocumentType>(
                    query.HasMoreResults ? page.ResponseContinuation : null,
                    page.ToArray());

            return pageResult;
        }

        public async Task<TDocumentType> GetDocumentAsync<TDocumentType>(string collection, Expression<Func<TDocumentType, bool>> selector)
        {
            var collectionUri = GetCollectionUri(collection);

            var options = new FeedOptions
            {
                MaxItemCount = 1
            };

            var query = _client.Value
                .CreateDocumentQuery<TDocumentType>(collectionUri, options)
                .Where(selector)
                .AsDocumentQuery();

            var item = await query.ExecuteNextAsync<TDocumentType>();

            return item.Single();
        }

        public async Task DeleteDocumentsAsync<TDocumentType>(string collection, Expression<Func<TDocumentType, bool>> selector)
        {
            var collectionUri = GetCollectionUri(collection);

            var options = new FeedOptions
            {
                MaxItemCount = 1
            };

            var query = _client.Value
                .CreateDocumentQuery<TDocumentType>(collectionUri, options)
                .Where(selector)
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                Parallel.ForEach(await query.ExecuteNextAsync<Document>(),
                    document =>
                    {
                        _client.Value.DeleteDocumentAsync(document.SelfLink);
                    });
            }
        }

        public async Task RecreateCollection(string collection)
        {
            var collectionUri = GetCollectionUri(collection);

            await _client.Value.DeleteDocumentCollectionAsync(collectionUri);

            await EnsureCollectionExists(collection);
        }

        private Uri EnsureCollectionAndGetUri(string collectionName)
        {
            RunWithTimeOut(EnsureCollectionExists(collectionName));

            return GetCollectionUri(collectionName);
        }

        private Uri GetCollectionUri(string collectionName)
        {
            return _collections.GetOrAdd(collectionName, name => UriFactory.CreateDocumentCollectionUri(_config.CosmosDatabase, name));
        }

        private readonly ConcurrentDictionary<string, object> _collectionsKnownToExists = new ConcurrentDictionary<string, object>();

        private Task EnsureCollectionExists(string collectionName)
        {
            var databaseUri = GetDatabaseUri();

            var collection = _documentCollectionConfigurator.ConfigureCollection(collectionName);

            return Client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection);
        }

        private Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(_config.CosmosDatabase);
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
                },
                ConsistencyLevel = ConsistencyLevel.Strong
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