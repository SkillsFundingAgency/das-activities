using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.ResultLogger;
using StructureMap;
using StructureMap.TypeRules;

namespace PerformanceTester.Types.IoC
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IActivityFactory>().Use<ActivityFactory>().Singleton();
            For<IConfigProvider>().Use<ConfigProvider>().Singleton();
            For<IStoreRepository>().Use<StoreRespository>().Singleton();

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("PerformanceTester"));
                scan.Include(type => type.IsConcreteAndAssignableTo(typeof(IResultLogger)));
                scan.With(new SingletonConvention<IResultLogger>());
            });

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("PerformanceTester"));
                scan.Include(type => type.IsConcreteAndAssignableTo(typeof(IStore)));
                scan.With(new SingletonConvention<IStore>());
            });

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("PerformanceTester"));
                scan.AddAllTypesOf<ICommand>();
            });
        }
    }
}
