using System.Threading.Tasks;

namespace SFA.DAS.Activities.Client.Elastic
{
    public interface IIndexAutoMapper
    {
        Task EnureIndexExists<T>(string indexName) where T : class;
    }
}