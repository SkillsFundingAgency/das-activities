namespace SFA.DAS.Activities.Configuration
{
    public interface ISettingsBuilder
    {
        SettingsBuilder AddProvider(IProvideSettings provider);
        SettingsProvider Build();
    }
}