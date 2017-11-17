using SFA.DAS.Configuration;

namespace SFA.DAS.Activities.Domain.Configurations
{
    public class ActivitiesConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string MessageServiceBusConnectionString { get; set; }
<<<<<<< HEAD:src/SFA.DAS.Activities.Domain/Configurations/ActivitiesConfiguration.cs

        public string ElasticServerBaseUrl { get; set; }
=======
>>>>>>> master:src/SFA.DAS.Activities.Application/Configurations/ActivitiesConfiguration.cs
    }
}
