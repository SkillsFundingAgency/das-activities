namespace SFA.DAS.Activities.Configuration
{
    public class ElasticConfiguration
    {
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IndexFormat { get; set; }
        public bool RequiresAuthentication => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
    }
}