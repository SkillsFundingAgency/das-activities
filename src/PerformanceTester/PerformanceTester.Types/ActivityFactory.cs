using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PerformanceTester.Types.Interfaces;

namespace PerformanceTester.Types
{
    public class ActivityFactory : IActivityFactory
    {
        private readonly ActivityType[] _activityTypes;
        private readonly Random _random;


        public ActivityFactory()
        {
            _activityTypes = Enum.GetValues(typeof(ActivityType)).Cast<ActivityType>().ToArray();
            _random = new Random(DateTime.Now.Millisecond);
        }

        public Activity CreateRandomActivity(long accountId, DateTime at)
        {
            var activityType = GetRandomAcvtivity();

            return new Activity
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                At = at,
                Created = DateTime.UtcNow,
                Data = new Dictionary<string, string>{
                    { "Property1", "value1"},
                    { "Property2", "value2"},
                    { "Property3", "value3"}
                },
                Description = $"Test {activityType} at {at:G}",
                Type = activityType
            };
        }

        public IEnumerable<AccountActivities> CreateActivitiesByAccount(PopulateActivitiesParameters populateActivitiesParameters)
        {
            for (int i = 0; i < populateActivitiesParameters.NumberOfAccountsRequired; i++)
            {
                var accountId = populateActivitiesParameters.FirstAccountNumber + i;

                var activitiesForThisAccount = CreateActivitiesForAccount(
                    accountId, 
                    populateActivitiesParameters.NumberOfActivitgiesPerAccount, 
                    populateActivitiesParameters.NumberOfActivitiesPerDay).ToList();

                var result = new AccountActivities
                {
                    AccountId = accountId,
                    Activites = activitiesForThisAccount
                };

                yield return result;
            }
        }

        public IEnumerable<Activity> CreateActivities(PopulateActivitiesParameters populateActivitiesParameters)
        {
            return CreateActivitiesByAccount(populateActivitiesParameters).SelectMany(abya => abya.Activites);
        }

        private IEnumerable<Activity> CreateActivitiesForAccount(long accountId, int activitiesRequired, int activitiesPerDay)
        {
            var atDate = DateTime.Today;
            var atTime = atDate;

            for (int i = 0; i < activitiesRequired; i++)
            {
                if (i % activitiesPerDay == 0)
                {
                    atDate = atDate.AddDays(-1);
                    atTime = atDate;
                }

                atTime = atTime.AddMinutes(-1);

                var activity = CreateRandomActivity(accountId, atTime);
                yield return activity;
            }
        }

        private ActivityType GetRandomAcvtivity()
        {
            return _activityTypes[_random.Next(0, _activityTypes.Length - 1)];
        }
    }
}