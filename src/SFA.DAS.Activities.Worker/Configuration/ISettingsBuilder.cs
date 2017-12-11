namespace SFA.DAS.Activities.Worker.Configuration
{
    public interface ISettingsBuilder
    {
        SettingsBuilder AddProvider(IProvideSettings provider);
        SettingsProvider Build();
    }
}