namespace SFA.DAS.Activities.Client
{
    public class ActivitiesClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
    }
}