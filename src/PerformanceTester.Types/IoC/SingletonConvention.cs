using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace PerformanceTester.Types.IoC
{
    public class SingletonConvention<TPluginFamily> : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.AllTypes())
            {
                if (type.CanBeCreated())
                {
                    registry.For(typeof(TPluginFamily)).Singleton().Use(type);
                }
            }
        }
    }
}