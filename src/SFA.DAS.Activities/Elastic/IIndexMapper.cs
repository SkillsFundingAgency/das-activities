using System.Threading.Tasks;
using Nest;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.Elastic
{
    public interface IIndexMapper
    {
        Task EnureIndexExists(IElasticClient client, ILog logger);
    }
}