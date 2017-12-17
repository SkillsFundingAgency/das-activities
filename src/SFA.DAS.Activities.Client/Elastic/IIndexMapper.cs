using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Activities.Client.Elastic
{
    public interface IIndexMapper
    {
        Task EnureIndexExists(IElasticClient client);
    }
}