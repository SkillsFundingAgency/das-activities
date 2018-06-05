using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace SFA.DAS.Activities
{
    public class CosmosClientQueryResult<TDocumentType>
    {
        public CosmosClientQueryResult(string continuationToken, TDocumentType[] items)
        {
            ContinuationToken = continuationToken;
            Items = items;
        }

        public string ContinuationToken { get; }
        public TDocumentType[] Items { get; set; }
    }

    public interface ICosmosClient
    {
        DocumentClient Client { get; }
        Task UpsertDocumentAsync(string collection, object entity);


        /// <summary>
        ///     Returns the first page of a set of results (or next page if <see cref="continuationToken"/> has a value).
        /// </summary>
        /// <typeparam name="TDocumentType">The type of document that is to be returned.</typeparam>
        /// <typeparam name="TKey">The type of the field being ordered by.</typeparam>
        /// <param name="collection">The collection being queried</param>
        /// <param name="continuationToken">If supplied indicates that the next page of results is required. The continuation token is provided by <see cref="CosmosClientQueryResult{TDocumentType}.ContinuationToken"/></param>
        /// <param name="orderby">The order in which the results are required.</param>
        /// <param name="pageSize">The size of page results that are required.</param>
        /// <returns></returns>
        Task<CosmosClientQueryResult<TDocumentType>> GetPageAsync<TDocumentType, TKey>(
            string collection,
            string continuationToken, 
            Expression<Func<TDocumentType, TKey>> orderby, 
            int pageSize);

        Task<TDocumentType> GetDocumentAsync<TDocumentType>(string collection, Expression<Func<TDocumentType, bool>> selector);

        Task DeleteDocumentsAsync<TDocumentType>(string collectionName, Expression<Func<TDocumentType, bool>> selector);
        Task RecreateCollection(string collectionName);
    }
}