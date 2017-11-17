namespace SFA.DAS.Activities.Application.Configurations
{
    public interface IActivitiesConfiguration
    {
        string ElasticServerBaseUrl { get; }
        string ServiceBusConnectionString { get; }
    }
}