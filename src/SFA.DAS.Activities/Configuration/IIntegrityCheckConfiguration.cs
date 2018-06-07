namespace SFA.DAS.Activities.Configuration
{
    public interface IIntegrityCheckConfiguration
    {
        int CosmosPageSize { get; }
        int ElasticPageSize { get; }
        int? MaxInspections { get; set; }
        int? MaxDiscrepancies { get; set; }
    }
}