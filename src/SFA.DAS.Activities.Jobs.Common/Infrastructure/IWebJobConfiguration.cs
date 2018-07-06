namespace SFA.DAS.Activities.Jobs.Common.Infrastructure
{
    public interface IWebJobConfiguration
    {
        string DashboardConnectionString { get; set; }
        string StorageConnectionString { get; set; }
    }
}