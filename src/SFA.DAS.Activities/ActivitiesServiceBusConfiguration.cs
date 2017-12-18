using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities
{
    public class ActivitiesServiceBusConfiguration : IServiceBusConfiguration
    {
        public string ConnectionString { get; set; }
    }
}