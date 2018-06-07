namespace SFA.DAS.Activities.IntegrityChecker.Interfaces
{
    public interface IFixActionReaderLogger
    {
        void IncrementCosmosActivities(int newActivitiesFound);
        void IncrementElasticActivities(int newActivitiesFound);
    }
}