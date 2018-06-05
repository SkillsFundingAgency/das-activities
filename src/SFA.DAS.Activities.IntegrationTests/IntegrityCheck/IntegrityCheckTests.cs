using System;
using System.IO;
using System.Threading.Tasks;
using Nest;
using NUnit.Framework;
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
        }

        [Test]
        public async Task CreatedActivitiesShouldAppearInBothElasticAndCosmos()
        {
            var fixtures = new IntegrityCheckTestFixtures();

            await fixtures.CreateActivities(50);

            await fixtures.AssertAllCreatedActivitiesAppearInCosmos();
            await fixtures.AssertAllCreatedActivitiesAppearInElastic();
        }

        [Test]
        public async Task ActivitiesMissingInCosmosShouldBeRecreatedFromElastic()
        {
            // arrange
            var fixtures = new IntegrityCheckTestFixtures();

            var deleteTasks = new Task[]
            {
                fixtures.DeleteAllActivitiesFromCosmos(),
                fixtures.DeleteAllActivitiesFromElastic()
            };

            await Task.WhenAll(deleteTasks);
            await fixtures.CreateActivities(10);
            var randomActivities = fixtures.GetRandomPostedActivities(5);
            await fixtures.DeleteActivitiesFromCosmos(randomActivities);

            // Act
            await fixtures.RunIntegrityCheck();

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInCosmos();
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
            await fixtures.RunIntegrityCheck();

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
            await fixtures.RunIntegrityCheck();

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
            await fixtures.RunIntegrityCheck();

            // Assert
            await fixtures.AssertAllCreatedActivitiesAppearInElastic();
        }
    }
}