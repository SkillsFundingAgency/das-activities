using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace PerformanceTester.ElasticDb.Interfaces
{
    public interface IIndexMapper : IDisposable
    {
        Task EnureIndexExistsAsync(string environmentName, IElasticClient client);
    }
}
