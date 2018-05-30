namespace SFA.DAS.Activities.Configuration
{

    public interface IElasticConfiguration
    {
        string ElasticUrl { get; set; }
        string ElasticUsername { get; set; }
        string ElasticPassword { get; set; }
    }

    public interface ICosmosConfiguration
    {
        string CosmosDatabase { get; set; }
        string CosmosEndpointUrl { get; set; }
        string CosmosPrimaryKey { get; set; }
        string CosmosCollectionName { get; set; }
    }
}