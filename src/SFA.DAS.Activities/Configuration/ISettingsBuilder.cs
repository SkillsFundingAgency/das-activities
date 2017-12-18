namespace SFA.DAS.Activities.Configuration
{
    public interface ISettingsBuilder
    {
        SettingsBuilder AddProvider(ISettingsProvider provider);
        Settings Build();
    }
}