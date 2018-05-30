namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public interface IWebJobConfiguration
    {
        string DashboardConnectionString { get; set; }
        string StorageConnectionString { get; set; }
    }
}