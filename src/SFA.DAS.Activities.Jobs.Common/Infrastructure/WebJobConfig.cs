namespace SFA.DAS.Activities.Jobs.Common.Infrastructure
{
    public class WebJobConfig : IWebJobConfiguration
    {
        public string DashboardConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
    }
}