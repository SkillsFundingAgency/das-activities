using SFA.DAS.Activities.Application.Configurations;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public interface ISettingsBuilder
    {
        SettingsBuilder AddProvider(IProvideSettings provider);
        SettingsProvider Build();
    }
}