using System.Threading.Tasks;
using Nest;
using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Elastic
{
    public interface IIndexMapper
    {
        Task EnureIndexExists(IEnvironmentConfiguration config, IElasticClient client);
    }
}