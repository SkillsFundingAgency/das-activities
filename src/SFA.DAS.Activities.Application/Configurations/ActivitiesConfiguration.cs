
namespace SFA.DAS.Activities.Application.Configurations
{
    public class ActivitiesConfiguration : IActivitiesConfiguration
    {
        private readonly IProvideSettings _settings;

        public ActivitiesConfiguration(IProvideSettings settings)
        {
            _settings = settings;
        }

        public string ElasticServerBaseUrl => _settings.GetSetting("ElasticSearch:BaseUrl");
        public string ServiceBusConnectionString => _settings.GetSetting("ServiceBus:ConnectionString");
    }
}
