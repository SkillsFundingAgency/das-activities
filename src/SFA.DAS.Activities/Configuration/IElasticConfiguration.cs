namespace SFA.DAS.Activities.Configuration
{
    public interface IElasticConfiguration
    {
        string ElasticUrl { get; set; }
        string ElasticUsername { get; set; }
        string ElasticPassword { get; set; }
    }
}