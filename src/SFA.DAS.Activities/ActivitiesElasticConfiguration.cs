namespace SFA.DAS.Activities
{
    public class ActivitiesElasticConfiguration
    {
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
    }
}