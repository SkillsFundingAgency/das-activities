using System;
using PerformanceTester.CosmosDb.IoC;
using PerformanceTester.ElasticDb.IoC;
using PerformanceTester.Types.IoC;
using StructureMap;

namespace PerformanceTester
{
    public static class IoC
    {
        public static IContainer InitializeIoC()
        {
            return new Container(c =>
            {
                c.AddRegistry<CommonRegistry>();
                c.AddRegistry<CosmosDbRegistry>();
                c.AddRegistry<ElasticRegistry>();
            });
        }
    }
}