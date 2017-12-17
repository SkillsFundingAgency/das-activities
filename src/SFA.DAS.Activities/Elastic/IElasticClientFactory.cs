using Nest;

namespace SFA.DAS.Activities.Elastic
{
    public interface IElasticClientFactory
    {
        IElasticClient GetClient();
    }
}