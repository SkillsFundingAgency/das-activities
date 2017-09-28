
namespace NuGetProject.Environment
{
    public class MachineEnvironment
    {
        public MachineEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }
        public string EnvironmentName { get; }
    }
}
