using SFA.DAS.Acitivities.Core.Configuration;

namespace SFA.DAS.Activities.Infrastructure.Configuration
{
    public interface ISettingsProvider
    {
        IOptions<T> GetSection<T>(string name) where T : new();
    }
}