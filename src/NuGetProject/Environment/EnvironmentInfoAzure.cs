using Microsoft.Azure;

namespace NuGetProject.Environment
{
    public class EnvironmentInfoAzure : IEnvironmentInfo
    {
        public MachineEnvironment GetEnvironment()
        {
            return new MachineEnvironment(CloudConfigurationManager.GetSetting("EnvironmentName"));
        }
    }
}
