namespace SFA.DAS.Activities.Configuration
{
    public interface ISettings
    {
        T GetSection<T>(string name) where T : new();
    }
}