using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Activities.IntegrityChecker;
using SFA.DAS.Activities.IntegrityChecker.Utils;

namespace SFA.DAS.Activities.UnitTests.IntegrityChecker.Utils
{
    [TestFixture]
    public class ActivityDiscrepancyQueueTests
    {
        [Test]
        public void StartProcessing_WithSomeMessages_AllMessagesShouldBeProcessedAndTheQueueShouldCloseDown()
        {
            new ActivityDiscrepancyQueueTestFixtures()
                .PushMessages(ActivityDiscrepancyType.NotFoundInElastic, "COS1", "COS2", "COS3")
                .PushMessages(ActivityDiscrepancyType.NotFoundInCosmos, "ES1", "ES2", "ES3")
                .StartProcessingQueue()
                .PushMessages(ActivityDiscrepancyType.NotFoundInElastic, "COS4", "COS5", "COS6")
                .PushMessages(ActivityDiscrepancyType.NotFoundInCosmos, "ES2", "ES3", "ES4")
                .CompleteAndWaitForQueueToBeProcessed()
                .AssertExpectedNumberOfMessagesProcessed(12)
                .AssertAllPushedMessagesHandled();
        }
    }

    class ActivityDiscrepancyQueueTestFixtures
    {
        private readonly ConcurrentStack<ActivityDiscrepancy> _handledMessages = new ConcurrentStack<ActivityDiscrepancy>();
        private Task _processingQueueTask;


        public ActivityDiscrepancyQueueTestFixtures()
        {
            PushedMessages = new List<ActivityDiscrepancy>();    
            ActivityDiscrepancyQueue = new ActivityDiscrepancyQueue();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public ActivityDiscrepancyQueue ActivityDiscrepancyQueue { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public List<ActivityDiscrepancy> PushedMessages { get; }


        public ActivityDiscrepancyQueueTestFixtures PushMessages(ActivityDiscrepancyType issue, params string[] activityIds)
        {
            foreach (var activityDiscrepancy in BuildDiscrepancies(issue, activityIds))
            {
                ActivityDiscrepancyQueue.Push(activityDiscrepancy);
                PushedMessages.Add(activityDiscrepancy);
            }

            return this;
        }

        public ActivityDiscrepancyQueueTestFixtures StartProcessingQueue()
        {
            _processingQueueTask = ActivityDiscrepancyQueue.StartQueueProcessingAsync(LogMessage, CancellationTokenSource.Token);
            return this;
        }

        public ActivityDiscrepancyQueueTestFixtures CompleteAndWaitForQueueToBeProcessed()
        {
            if (_processingQueueTask == null)
            {
                throw new InvalidOperationException($"Cannot wait for queue to complete because {nameof(StartProcessingQueue)} has not been called yet");
            }

            ActivityDiscrepancyQueue.AddComplete();
            if (!_processingQueueTask.Wait(5 * 1000))
            {
                Assert.Fail("The queue processor did not finish in a timely manner");
            }

            return this;
        }

        public ActivityDiscrepancyQueueTestFixtures AssertAllPushedMessagesHandled()
        {
            Assert.AreEqual(PushedMessages.Count, _handledMessages.Count);
            foreach (var message in PushedMessages)
            {
                Assert.IsTrue(_handledMessages.Contains(message));
            }
            return this;
        }

        public ActivityDiscrepancyQueueTestFixtures AssertExpectedNumberOfMessagesProcessed(int expectedNumberOfMessages)
        {
            Assert.AreEqual(expectedNumberOfMessages, PushedMessages.Count);
            return this;
        }

        private IEnumerable<ActivityDiscrepancy> BuildDiscrepancies(ActivityDiscrepancyType issue, params string[] activityIds)
        {
            return activityIds.Select(id => new ActivityDiscrepancy(new Activity
            {
                AccountId = 123,
                At = DateTime.Now,
                Id = id,
                Created = DateTime.Now,
                Description = $"Msg.{id}",
                Type = ActivityType.AccountCreated
            }, issue));
        }

        private void LogMessage(ActivityDiscrepancy activityDiscrepancy)
        {
            _handledMessages.Push(activityDiscrepancy);
        }
    }
}
