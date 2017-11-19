
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
        public string ElasticSearchUserName => _settings.GetNullableSetting("ElasticSearch:UserName");
        public string ElasticSearchPassword => _settings.GetNullableSetting("ElasticSearch:Password");
        public string ElasticSearchIndexFormat => _settings.GetSetting("ElasticSearch:IndexFormat");

        public bool RequiresAuthentication => !string.IsNullOrEmpty(ElasticSearchUserName) &&
                                              string.IsNullOrEmpty(ElasticSearchPassword);

        public string EnvironmentName => _settings.GetSetting("EnvironmentName");
    }
}
