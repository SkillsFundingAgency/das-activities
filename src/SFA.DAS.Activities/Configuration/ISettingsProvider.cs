namespace SFA.DAS.Activities.Configuration
{
    public interface ISettingsProvider
    {
        T GetSection<T>(string name) where T : new();
    }
}