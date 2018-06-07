using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
using SFA.DAS.Activities.Worker;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.IntegrityChecker.Worker;
using SFA.DAS.IntegrityChecker.Worker.CreateActivities;
using SFA.DAS.IntegrityChecker.Worker.Infrastructure;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.Activities.IntegrationTests.IntegrityCheck
{
    class PostedActivityResult
    {
        public Activity Activity { get; set; }
        public Exception Exception { get; set; }
        public bool Successful => Exception == null;
        public bool DeletedFromCosmos { get; set; }
        public bool DeletedFromElastic { get; set; }
    }

    class IntegrityCheckTestFixtures
    {
        // We have two IoC because this test does two things:
        //    1. creates activities, which is a function of the ActivitiesWorker and so depends on its IoC setup, and 
        //    2. runs the integrity-check, which is a function of the IntegrityCheckerWorker and so depends on its IoC setup
        // If we merge the two IoC so we have a super-set IoC we might inadvertently cause something to work because an IoC hole
        // has been plugged, or we might cause something to fail because one IoC registration conflicts with the other. 
        private readonly Lazy<IContainer> _activityWorkerContainer;
        private readonly Lazy<IContainer> _integrityWorkerContainer;

        public IActivitySaver ActivitySaver => GetActivityWorkerService<IActivitySaver>();
        public IMessageContextProvider MessageContextProvider => GetActivityWorkerService<IMessageContextProvider>();
        public ICosmosActivityDocumentRepository CosmosActivityDocumentRepository => GetIntegrityCheckerWorkerService<ICosmosActivityDocumentRepository>();
        public IElasticActivityDocumentRepository ElasticActivityDocumentRepository => GetIntegrityCheckerWorkerService<IElasticActivityDocumentRepository>();
        public CancellationTokenSource CancellationTokenSource { get; }

        public IntegrityCheckTestFixtures()
        {
            CreatedActivities = new ConcurrentBag<Activity>();    
            CancellationTokenSource = new CancellationTokenSource();
            _activityWorkerContainer = new Lazy<IContainer>(InitialiseActivityWorkerIoC);
            _integrityWorkerContainer = new Lazy<IContainer>(InitialiseIntegrityCheckerIoC);
        }

        public IContainer ActivityWorkerContainer => _activityWorkerContainer.Value;

        public IContainer IntegrityCheckerContainer => _integrityWorkerContainer.Value;

        public IntegrityCheckerJob CreateIntegrityCheckJob()
        {
            return new IntegrityCheckerJob();
        }

        public async Task<FixActionLogger> RunIntegrityCheck(string lognameSuffix)
        {
            await RefreshElasticIndex();

            var job = ServiceLocator.Get<IntegrityChecker.IntegrityCheck>();
            var logger = await job.DoAsync(CancellationTokenSource.Token, lognameSuffix);

            await RefreshElasticIndex();
            return logger;
        }

        public Task CreateActivities(int number)
        {
            var tasks = new Task[number];

            for (int i = 0; i < number; i++)
            {
                AccountCreatedMessage message = new AccountCreatedMessage(i, $"User {i}", "UserRef {i}");
                var messageContext = new FakeMessage<AccountCreatedMessage>(message, Guid.NewGuid().ToString());

                MessageContextProvider.StoreMessageContext(messageContext);

                tasks[i] = ActivitySaver
                        .SaveActivity(message, ActivityType.AccountCreated)
                        .ContinueWith(t => CreatedActivities.Add(t.Result))
                        .ContinueWith(t => MessageContextProvider.ReleaseMessageContext(messageContext));
            }

            return Task.WhenAll(tasks)
                .ContinueWith(task =>
                {
                    Assert.IsTrue(tasks.All(t => t.IsCompleted), "At least one creation task did not complete");

                    var firstFaulted = tasks.FirstOrDefault(t => t.IsFaulted);
                    if (firstFaulted != null)
                    {
                        Assert.Fail($"At least one creation task faulted.{firstFaulted.Exception.Flatten().Message}");
                    }

                    Assert.AreEqual(number, CreatedActivities.Count, "Incorrect number of accounts created");
                });
        }

        public Task RefreshElasticIndex()
        {
            var client = GetIntegrityCheckerWorkerService<IElasticClient>();
            return client.RefreshAsync("local-activities");
        }

        public Task DeleteActivitiesFromCosmos(int activitiesToRemoveFromCosmos)
        {
            var randomActivities = GetRandomPostedActivities(activitiesToRemoveFromCosmos);
            return DeleteActivitiesFromCosmos(randomActivities);
        }

        public Task DeleteActivitiesFromCosmos(Activity[] activities)
        {
            return DeleteActivitiesFromRepo(CosmosActivityDocumentRepository, activities);
        }

        public Task DeleteActivitiesFromElastic(Activity[] activities)
        {
            return DeleteActivitiesFromRepo(ElasticActivityDocumentRepository, activities);
        }

        public Task DeleteAllExistingActivitiesFromElasticAndCosmos()
        {
            return Task.WhenAll(new[]
            {
                DeleteAllActivitiesFromCosmos(),
                DeleteAllActivitiesFromElastic()
            });
        }
        public Task DeleteAllActivitiesFromCosmos()
        {
            return CosmosActivityDocumentRepository.DeleteAllActivitiesFromRepoAsync();
        }

        public Task DeleteAllActivitiesFromElastic()
        {
            return ElasticActivityDocumentRepository.DeleteAllActivitiesFromRepoAsync();
        }

        public void AssertServiceCanBeConstructed<T>(bool inActivityWorker, bool inIntegrityChecker)
        {
            try
            {
                if (inActivityWorker)
                {
                    AssertService(GetActivityWorkerService<T>, "activity");
                }

                if (inIntegrityChecker)
                {
                    AssertService(GetIntegrityCheckerWorkerService<T>, "integrity checker");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task AssertAllCreatedActivitiesAppearInCosmos()
        {
            return AssertAllCreatedActivitiesAppearInRepo(CosmosActivityDocumentRepository);
        }

        public Task AssertAllCreatedActivitiesAppearInElastic()
        {
            return AssertAllCreatedActivitiesAppearInRepo(ElasticActivityDocumentRepository);
        }

        public ConcurrentBag<Activity> CreatedActivities { get; }

        public Activity[] GetRandomPostedActivities(int numberRequired)
        {
            if (numberRequired < 0 || numberRequired > CreatedActivities.Count)
            {
                throw new ArgumentOutOfRangeException($"Can only pick a random selection of posted activities between 1 and the number of activities that have been posted (which is {CreatedActivities.Count}).");
            }

            if (numberRequired == 0)
            {
                return new Activity[0];
            }

            var availableActivities = CreatedActivities.ToArray();

            var selectedActivities = new Activity[numberRequired];

            var selectionIdx = 0;
            var random = new Random();

            while (selectionIdx < numberRequired)
            {
                var activityIdx = random.Next(0, CreatedActivities.Count);

                if (availableActivities[activityIdx] != null)
                {
                    selectedActivities[selectionIdx] = availableActivities[activityIdx];
                    availableActivities[activityIdx] = null;
                    selectionIdx++;
                }
            }

            return selectedActivities;
        }

        public T GetIntegrityCheckerWorkerService<T>()
        {
            return IntegrityCheckerContainer.GetInstance<T>();
        }

        public T GetActivityWorkerService<T>()
        {
            return ActivityWorkerContainer.GetInstance<T>();
        }

        private IContainer InitialiseActivityWorkerIoC()
        {
            var container = ActivityWorker.InitializeIoC();
            return container;
        }

        private IContainer InitialiseIntegrityCheckerIoC()
        {

            var container = WebJob.InitializeIoC();
            ServiceLocator.Initialise(container);
            return container;
        }

        private Task DeleteActivitiesFromRepo(IActivityDocumentRepository repo, Activity[] activities)
        {
            var tasks = new Task[activities.Length];

            Parallel.For(0, activities.Length, i =>
            {
                tasks[i] = repo.DeleteActivityAsync(activities[i].Id);
            });

            return Task.WhenAll(tasks);
        }

        private async Task AssertAllCreatedActivitiesAppearInRepo(IActivityDocumentRepository repo)
        {
            foreach (var postedActivity in CreatedActivities)
            {
                var activity = await repo.GetActivityAsync(postedActivity.Id);
                Assert.IsNotNull(activity, $"Did not find activity with id {postedActivity.Id} in {repo.GetType().FullName}");
            }
        }

        private void AssertService<T>(Func<T> serviceGetter, string containerName)
        {
            try
            {
                var t = serviceGetter();
                if (t == null)
                {
                    throw new NullReferenceException("Service getter function returned null");
                }
            }
            catch (Exception e)
            {
                throw new AssertionException($"Could not get an instance of {typeof(T)} from {containerName} container.{Environment.NewLine}{e.Message}", e);
            }
        }
    }
}