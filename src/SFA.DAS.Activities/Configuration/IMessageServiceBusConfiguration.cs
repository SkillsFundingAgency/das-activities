namespace SFA.DAS.Activities.Configuration
{
    public interface IMessageServiceBusConfiguration
    {
        string MessageServiceBusConnectionString { get; set; }
        string CosmosDatabase { get; set; }
        string CosmosEndpointUrl { get; set; }
        string CosmosPrimaryKey { get; set; }
        string CosmosCollectionName { get; set; }
    }
}