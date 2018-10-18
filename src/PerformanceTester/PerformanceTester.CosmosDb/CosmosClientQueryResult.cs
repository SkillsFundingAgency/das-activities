using PerformanceTester.Types.Types;

namespace PerformanceTester.CosmosDb
{
    public class CosmosClientQueryResult<TDocumentType>
    {
        public CosmosClientQueryResult(string continuationToken, TDocumentType[] items)
        {
            this.ContinuationToken = continuationToken;
            this.Items = items;
        }

        public string ContinuationToken { get; }

        public TDocumentType[] Items { get; set; }

        public OperationCost Cost { get; set; }
    }
}
