namespace SFA.DAS.Activities.Configuration
{
    public interface IElasticConfiguration
    {
        string BaseUrl { get; }
        string UserName { get; }
        string Password { get; }
        bool RequiresAuthentication { get; }
    }
}