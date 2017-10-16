using SFA.DAS.Configuration;

namespace SFA.DAS.Activities.Application.Configurations
{
    public class ActivitiesConfiguration : IConfiguration
    {
        public string ElasticServerBaseUrl { get; set; } = "\"http://localhost:9200\"";
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
