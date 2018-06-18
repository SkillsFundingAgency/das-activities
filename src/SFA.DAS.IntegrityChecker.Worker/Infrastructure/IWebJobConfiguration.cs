namespace SFA.DAS.Activities.Jobs.Infrastructure
{
    public interface IWebJobConfiguration
    {
        string DashboardConnectionString { get; set; }
        string StorageConnectionString { get; set; }
    }
}