using SFA.DAS.Activities.Client.TestHost.Config;
using SFA.DAS.Activities.Client.TestHost.Interfaces;
using SFA.DAS.Activities.Client.TestHost.ResultSavers;
using StructureMap;

namespace SFA.DAS.Activities.Client.TestHost.IoC
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IConfigProvider>().Use<ConfigProvider>().Singleton();
            For<IResultSaver>().Use<FileResultSaver>();

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("TestHost"));
                scan.AddAllTypesOf<ICommand>();
            });
        }
    }
}