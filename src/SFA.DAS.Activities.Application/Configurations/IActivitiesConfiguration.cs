namespace SFA.DAS.Activities.Application.Configurations
{
    public interface IActivitiesConfiguration
    {
        string ElasticServerBaseUrl { get; }
        string ServiceBusConnectionString { get; }
        string ElasticSearchUserName { get; }
        string ElasticSearchPassword { get; }
        string ElasticSearchIndexFormat { get; }
        bool RequiresAuthentication { get; }
        string EnvironmentName { get; }
    }
}