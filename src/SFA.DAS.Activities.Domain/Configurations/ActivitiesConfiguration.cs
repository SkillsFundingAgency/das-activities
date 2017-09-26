using SFA.DAS.Configuration;

namespace SFA.DAS.Activities.Domain.Configurations
{
    public class ActivitiesConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
