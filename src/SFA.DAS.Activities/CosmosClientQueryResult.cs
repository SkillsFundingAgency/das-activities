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
}