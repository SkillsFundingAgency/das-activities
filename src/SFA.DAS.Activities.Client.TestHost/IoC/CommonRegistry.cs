using SFA.DAS.Activities.Client.TestHost.Config;
using SFA.DAS.Activities.Client.TestHost.Interfaces;
using StructureMap;

namespace SFA.DAS.Activities.Client.TestHost.IoC
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IConfigProvider>().Use<ConfigProvider>().Singleton();

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("PerformanceTester"));
                scan.AddAllTypesOf<ICommand>();
            });
        }
    }
}