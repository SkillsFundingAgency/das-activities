namespace SFA.DAS.IntegrityChecker.Worker.Infrastructure
{
    public class WebJobConfig : IWebJobConfiguration
    {
        public string DashboardConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
    }
}