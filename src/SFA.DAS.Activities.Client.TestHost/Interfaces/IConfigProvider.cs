namespace SFA.DAS.Activities.Client.TestHost.Interfaces
{
    public interface IConfigProvider
    {
        TConfigType Get<TConfigType>() where TConfigType : class, new();
    }
}