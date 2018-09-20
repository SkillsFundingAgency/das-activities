using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PerformanceTester.Types;

namespace PerformanceTester.CosmosDb
{
    public class CosmosDbStore : IStore, IDisposable
    {
        private static readonly TimeSpan DefaultTimeOut = new TimeSpan(0, 0, 30);
        private readonly Lazy<DocumentClient> _client;
        private RequestOptions _requestOptions;
        private Uri _collectionUri;
        private readonly Lazy<CosmosConfig> _cosmosConfig;
        private const string CollectionName = "activities";

        private CosmosConfig CosmosConfig
        {
            get
            {
                return this._cosmosConfig.Value;
            }
        }

        private DocumentClient Client
        {
            get
            {
                return this._client.Value;
            }
        }

        public CosmosDbStore(IConfigProvider configProvider)
        {
            CosmosDbStore cosmosDbStore = this;
            this._client = new Lazy<DocumentClient>((Func<DocumentClient>)(() => cosmosDbStore.InitialiseClient(configProvider)));
            this._cosmosConfig = new Lazy<CosmosConfig>((Func<CosmosConfig>)(() => (CosmosConfig)configProvider.Get<CosmosConfig>()));
        }

        public string Name => "CosmosDB";

        public Task Initialise()
        {
            return this.EnsureCollectionExists("activities");
        }

        public Task<IOperationCost> PersistActivityAsync(Activity activity, CancellationToken cancellationToken)
        {
            return this.UpsertDocumentAsync(this.GetCollectionUri("activities"), (object)activity, cancellationToken);
        }

        public async Task<IOperationCost> GetActivitiesForAccountAsync(long accountId)
        {
            var result = new GroupOperationCost($"GetActivitiesForAccountAsync({accountId})");
            await FetchAllMatchingActivities(activity => activity.AccountId == accountId, result);
            return result;
        }

        private async Task<IOperationCost> UpsertDocumentAsync(Uri collectionUri, object entity, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var cosmosResponse = await this.Client.UpsertDocumentAsync(collectionUri, entity, this._requestOptions, false, cancellationToken);
            sw.Stop();
            return new OperationCost("Upsert activity", cosmosResponse.RequestCharge, sw.ElapsedMilliseconds);
        }

        private async Task FetchAllMatchingActivities(Expression<Func<Activity, bool>> selection, GroupOperationCost groupOperationCost)
        {
            CosmosClientQueryResult<Activity> page = null;
            do
            {
                page = await FetchPageOfActivities(selection, page?.ContinuationToken);
                groupOperationCost.StepCosts.Add(page.Cost);
            }
            while (page.ContinuationToken != null);
        }

        private async Task<CosmosClientQueryResult<Activity>> FetchPageOfActivities(Expression<Func<Activity, bool>> selection, string continuationToken)
        {
            var collectionUri = GetCollectionUri(CollectionName);

            var options = new FeedOptions
            {
                MaxItemCount = 1000,
                RequestContinuation = continuationToken
            };

            var query = _client.Value
                .CreateDocumentQuery<Activity>(collectionUri, options) 
                .OrderBy(activity => activity.At)
                .AsDocumentQuery();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var page = await query.ExecuteNextAsync<Activity>();
            sw.Stop();
            var pageResult = new CosmosClientQueryResult<Activity>(
                query.HasMoreResults ? page.ResponseContinuation : null, 
                page.ToArray())
                {
                    Cost = new OperationCost(query.ToString(), page.RequestCharge, sw.ElapsedMilliseconds)
                };

            return pageResult;
        }

        private DocumentClient InitialiseClient(IConfigProvider configProvider)
        {
            DocumentClient documentClient1 = new DocumentClient(new Uri(this.CosmosConfig.CosmosEndpointUrl), this.CosmosConfig.CosmosPrimaryKey, (ConnectionPolicy)null, new ConsistencyLevel?());
            RequestOptions requestOptions = new RequestOptions();
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            requestOptions.JsonSerializerSettings = serializerSettings;
            requestOptions.ConsistencyLevel = ConsistencyLevel.Strong;
            this._requestOptions = requestOptions;
            DocumentClient documentClient2 = documentClient1;
            Database database = new Database();
            database.Id = CosmosConfig.CosmosDatabase;
            this.RunWithTimeOut(documentClient2.CreateDatabaseIfNotExistsAsync(database, requestOptions));
            return documentClient1;
        }

        private Uri GetCollectionUri(string collectionName)
        {
            Uri uri = this._collectionUri;
            if ((object)uri == null)
                uri = this._collectionUri = UriFactory.CreateDocumentCollectionUri(this.CosmosConfig.CosmosDatabase, collectionName);
            return uri;
        }

        private Task EnsureCollectionExists(string collectionName)
        {
            return (Task)this.Client.CreateDocumentCollectionIfNotExistsAsync(this.GetDatabaseUri(), this.BuildDefaultCollection(collectionName), (RequestOptions)null);
        }

        public void Dispose()
        {
            if (!this._client.IsValueCreated)
                return;
            this._client.Value.Dispose();
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

        private Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(this.CosmosConfig.CosmosDatabase);
        }

        private void RunWithTimeOut(Task task)
        {
            this.RunWithTimeOut(task, CosmosDbStore.DefaultTimeOut);
        }

        private void RunWithTimeOut(Task task, TimeSpan timeout)
        {
            task.Wait(timeout);
            if (!task.IsCompleted)
                throw new Exception("Timed out waiting for operation to complete");
        }
    }
}
