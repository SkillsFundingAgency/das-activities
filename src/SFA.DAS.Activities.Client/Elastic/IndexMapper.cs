using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Activities.Client.Elastic
{
    public abstract class IndexMapper<T> : IIndexMapper where T : class
    {
        protected abstract string IndexName { get; }

        public async Task EnureIndexExists(IElasticClient client)
        {
            client.ConnectionSettings.DefaultIndices.Add(typeof(T), IndexName);

            var response = await client.IndexExistsAsync(IndexName).ConfigureAwait(false);

            if (!response.Exists)
            {
                await client.CreateIndexAsync(IndexName, i => i
                    .Mappings(ms => ms
                        .Map<T>(m =>
                        {
                            Map(m);
                            return m;
                        })
                    )
                );
            }
        }

        protected abstract void Map(TypeMappingDescriptor<T> mapper);
    }
}