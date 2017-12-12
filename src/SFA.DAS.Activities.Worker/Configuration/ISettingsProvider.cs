namespace SFA.DAS.Activities.Worker.Configuration
{
    public interface ISettingsProvider
    {
        T GetSection<T>(string name) where T : new();
    }
}