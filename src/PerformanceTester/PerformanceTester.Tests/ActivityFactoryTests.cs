using NUnit.Framework;
using PerformanceTester.Types;
using PerformanceTester.Types.Interfaces;
using PerformanceTester.Types.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester.Tests
{
    public class TestStore1 : TestStoreBase
    {
    }
    public class TestStore2 : TestStoreBase
    {
    }

    public class TestStore3 : TestStoreBase
    {
    }

        public abstract class TestStoreBase : IStore
        {
            protected TestStoreBase()
            {
                this.Name = this.GetType().Name;
            }

            public string Name { get; }

            public Task Initialise()
            {
                throw new NotImplementedException();
            }

            public Task<IOperationCost> PersistActivityAsync(Activity activity, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<IOperationCost> GetActivitiesForAccountAsync(long accoundId)
            {
                throw new NotImplementedException();
            }
        }

    [TestFixture]
    public class ActivityFactoryTests
    {
        [Test]
        public void CreateRandomActivity_ValidCall_ShouldReturnAnActivity()
        {
            Assert.IsNotNull((object)new ActivityFactory().CreateRandomActivity(1L, DateTime.Today));
        }

        [Test]
        public void CreateRandomActivity_ValidCall_ShouldSetAccountId()
        {
            Assert.AreEqual((object)1L, (object)new ActivityFactory().CreateRandomActivity(1L, DateTime.Today).AccountId);
        }

        [TestCase(1, 1, 1)]
        [TestCase(5, 10, 1)]
        [TestCase(5, 20, 5)]
        public void CreateActivities_ValidCall_ShouldCreateExpectedNumberOfActivities(int numberOfAccounts, int numberOfActivitiesPerAccount, int numberOfActivitiesPerDay)
        {
            List<AccountActivities> list = new ActivityFactory().CreateActivitiesByAccount(this.CreateParameters(numberOfAccounts, numberOfActivitiesPerAccount, numberOfActivitiesPerDay)).ToList<AccountActivities>();
            Assert.AreEqual((object)numberOfAccounts, (object)list.Count, "Incorrect number of accounts", Array.Empty<object>());
            foreach (AccountActivities accountActivities in list)
                Assert.AreEqual((object)numberOfActivitiesPerAccount, (object)accountActivities.Activites.Count, "Incorrect number of activities for account", Array.Empty<object>());
        }

        private PopulateActivitiesParameters CreateParameters(int numberOfAccounts, int numberOfActivitiesPerAccount, int numberOfActivitiesPerDay)
        {
            return new PopulateActivitiesParameters()
            {
                NumberOfAccountsRequired = numberOfAccounts,
                NumberOfActivitgiesPerAccount = numberOfActivitiesPerAccount,
                NumberOfActivitiesPerDay = numberOfActivitiesPerDay
            };
        }
    }
}
