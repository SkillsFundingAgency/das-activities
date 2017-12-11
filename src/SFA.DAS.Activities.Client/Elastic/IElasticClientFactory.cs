using Nest;

namespace SFA.DAS.Activities.Client.Elastic
{
    public interface IElasticClientFactory
    {
        IElasticClient GetClient();
    }
}