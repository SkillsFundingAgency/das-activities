
namespace NuGetProject.Environment
{
    public class EnvironmentInfoFile : IEnvironmentInfo
    {
        public MachineEnvironment GetEnvironment()
        {
            return new MachineEnvironment(System.Environment.GetEnvironmentVariable("DASENV"));
        }
    }
}
