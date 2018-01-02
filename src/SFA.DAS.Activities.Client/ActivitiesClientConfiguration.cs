using SFA.DAS.Activities.Configuration;

namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientConfiguration : IElasticConfiguration
    {
        public string ElasticUrl { get; set; }
        public string ElasticUsername { get; set; }
        public string ElasticPassword { get; set; }
    }
}