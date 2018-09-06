namespace SFA.DAS.Activities.Configuration
{
    public interface ICosmosConfiguration
    {
        string CosmosDatabase { get; set; }
        string CosmosEndpointUrl { get; set; }
        string CosmosPrimaryKey { get; set; }
        string CosmosCollectionName { get; set; }
    }
}