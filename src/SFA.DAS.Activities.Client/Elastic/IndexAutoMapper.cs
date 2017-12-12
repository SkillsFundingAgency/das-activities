using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Activities.Client.Elastic
{
    public class IndexAutoMapper : IIndexAutoMapper
    {
        private readonly List<string> _indexNames = new List<string>();
        private readonly IElasticClient _client;

        public IndexAutoMapper(IElasticClient client)
        {
            _client = client;
        }

        public async Task EnureIndexExists<T>(string indexName) where T : class
        {
            if (!_indexNames.Contains(indexName))
            {
                var response = await _client.IndexExistsAsync(indexName);

                if (!response.Exists)
                {
                    await _client.CreateIndexAsync(indexName, i => i.Mappings(ms => ms.Map<T>(m => m.AutoMap())));
                }

                _indexNames.Add(indexName);
            }
        }
    }
}