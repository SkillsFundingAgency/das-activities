using System;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Elastic
{
    public abstract class IndexMapper<T> : IIndexMapper where T : class
    {
        protected abstract string IndexName { get; }

        public async Task EnureIndexExists(IElasticClient client, ILog logger)
        {
            try
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
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to create '{IndexName}' index.");
                throw;
            }
        }

        protected abstract void Map(TypeMappingDescriptor<T> mapper);
    }
}