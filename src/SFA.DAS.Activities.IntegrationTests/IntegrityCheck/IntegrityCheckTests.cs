using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.ActivitySavers;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using StructureMap;

namespace SFA.DAS.Activities.IntegrationTests.IntegrityCheck
{
    [TestFixture]
    public class IntegrityCheckTests
    {
        public IntegrityCheckTests()
        {
            if (!EnvironmentHelper.IsAnyOf(DeployedEnvironment.Local, DeployedEnvironment.AT))
            {
                throw new Exception(
                    $"This set of integration tests are destructive - they cannot be run in environments where data loss is unacceptable. Current environment is {EnvironmentHelper.CurrentDeployedEnvironment}");
            }
        }

        [Test]
        public void RequiredServicesAreRegisteredInTheRelevantIoCCorrectly()
        {
            var fixtures = new IntegrityCheckTestFixtures();

            fixtures.AssertServiceCanBeConstructed<IElasticClient>(true, true);
            fixtures.AssertServiceCanBeConstructed<ICosmosClient>(true, true);
            fixtures.AssertServiceCanBeConstructed<ICosmosActivityDocumentRepository>(true, true);
            fixtures.AssertServiceCanBeConstructed<IElasticActivityDocumentRepository>(true, true);
            fixtures.AssertServiceCanBeConstructed<IActivitySaver>(true, false);
            fixtures.AssertServiceCanBeConstructed<IActivitiesScan>(false, true);
            fixtures.AssertServiceCanBeConstructed<IActivitiesFix>(false, true);
            fixtures.AssertServiceCanBeConstructed<IDocumentCollectionConfigurator>(true, true);
            fixtures.AssertServiceCanBeConstructed<IActivityDiscrepancyFinder>(false, true);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(20)]
        public async Task CreatedActivitiesShouldAppearInBothElasticAndCosmos(int activitiesToCreate)
        {
            var fixtures = new IntegrityCheckTestFixtures();

            await fixtures.CreateActivities(activitiesToCreate);

            var tasks = new []
            {
                fixtures.AssertAllCreatedActivitiesAppearInCosmos(),
                fixtures.AssertAllCreatedActivitiesAppearInElastic()
            };

            await Task.WhenAll(tasks);
        }

        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(5, 0)]
        [TestCase(5, 2)]
        [TestCase(5, 5)]
        public async Task ActivitiesMissingInCosmosShouldBeRecreatedFromElastic(int totalActivitiesRequired, int activitiesToRemoveFromCosmos)
        {
            // arrange
            var fixtures = new IntegrityCheckTestFixtures();

            await fixtures.DeleteAllExistingActivitiesFromElasticAndCosmos();

            await fixtures.CreateActivities(totalActivitiesRequired);

            await fixtures.DeleteActivitiesFromCosmos(activitiesToRemoveFromCosmos);

            await Task.Delay(10000);

            // Act
            await fixtures.RunIntegrityCheck($"_{totalActivitiesRequired}.{activitiesToRemoveFromCosmos}");
            fixtures.CancellationTokenSource.Cancel();

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInCosmos();
            int expectedNumberOfFixes = activitiesToRemoveFromCosmos;
            int actualNumberOfFixes = fixtures.FixActionLogger.GetFixes().Count();
            if (actualNumberOfFixes != expectedNumberOfFixes)
            {
                Assert.AreEqual(expectedNumberOfFixes, actualNumberOfFixes, "Incorrect number of fixes applied");
            }
        }

        [Test]
        public async Task ActivitiesMissingInCosmosShouldNotCauseThoseActivitiesToBeRemovedFromElastic()
        {
            // arrange
            var fixtures = new IntegrityCheckTestFixtures();

            var deleteTasks = new Task[]
            {
                fixtures.DeleteAllActivitiesFromCosmos(),
                fixtures.DeleteAllActivitiesFromElastic()
            };

            await Task.WhenAll(deleteTasks);
            await fixtures.CreateActivities(50);
            var randomActivities = fixtures.GetRandomPostedActivities(10);
            await fixtures.DeleteActivitiesFromCosmos(randomActivities);

            // Act
            await fixtures.RunIntegrityCheck(null);

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInElastic();
        }

        [Test]
        public async Task ActivitiesMissingInElasticShouldBeRecreatedFromCosmos()
        {
            // arrange
            var fixtures = new IntegrityCheckTestFixtures();

            var deleteTasks = new Task[]
            {
                fixtures.DeleteAllActivitiesFromCosmos(),
                fixtures.DeleteAllActivitiesFromElastic()
            };

            await Task.WhenAll(deleteTasks);
            await fixtures.CreateActivities(50);
            var randomActivities = fixtures.GetRandomPostedActivities(10);
            await fixtures.DeleteActivitiesFromCosmos(randomActivities);

            // Act
            await fixtures.RunIntegrityCheck(null);

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInElastic();
        }

        [Test]
        public async Task ActivitiesMissingInElasticShouldNotCauseThoseActivitiesToBeRemovedFromCosmos()
        {
            // arrange
            var fixtures = new IntegrityCheckTestFixtures();

            var deleteTasks = new Task[]
            {
                fixtures.DeleteAllActivitiesFromCosmos(),
                fixtures.DeleteAllActivitiesFromElastic()
            };

            await Task.WhenAll(deleteTasks);
            await fixtures.CreateActivities(50);
            var randomActivities = fixtures.GetRandomPostedActivities(10);
            await fixtures.DeleteActivitiesFromCosmos(randomActivities);

            // Act
            await fixtures.RunIntegrityCheck(null);

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInElastic();
        }
    }
}