using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.FixLogging;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.UnitTests.IntegrityChecker.FixLogging
{
    [TestFixture]
    public class FixActionLoggerTests
    {
        [Test]
        public void Finish_AfterFinish_StartTimeShouldBeSet()
        {
            var datetime = DateTime.UtcNow;
            CheckPropertyPostFinish(logger => Assert.GreaterOrEqual(logger.StartTime, datetime));
        }

        [Test]
        public void Finish_AfterFinish_EndTimeShouldBeSet()
        {
            var datetime = DateTime.UtcNow;
            CheckPropertyPostFinish(logger => Assert.GreaterOrEqual(logger.EndTime, logger.StartTime));
        }

        [TestCase(1, 0)]
        [TestCase(2, 0)]
        [TestCase(5, 0)]
        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(0, 5)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(5, 5)]
        public void Finish_AfterAddHandlers_FixFailuresAndSuccessesShouldBeSet(int numberOfFailures, int numberOfSuccesses)
        {
            var datetime = DateTime.UtcNow;
            CheckPropertyPostFinish(
                setup: logger => logger
                                    .AddFailedFixer(ActivityDiscrepancyType.NotFoundInCosmos, numberOfFailures)
                                    .AddSuccessFixer(ActivityDiscrepancyType.NotFoundInCosmos, numberOfSuccesses),
                assert: logger =>
                {
                    Assert.AreEqual(numberOfFailures, logger.FixFailures, "Failures is not correct");
                    Assert.AreEqual(numberOfSuccesses, logger.FixSuccesses, "Successes is not correct");
                });
        }

        [TestCase(1, 0, 0, 0)]
        [TestCase(0, 1, 0, 0)]
        [TestCase(0, 0, 1, 0)]
        [TestCase(0, 0, 0, 1)]
        [TestCase(5, 5, 0, 0)]
        [TestCase(0, 0, 5, 5)]
        [TestCase(5, 5, 5, 5)]
        public void Finish_AfterAddHandlers_FixHandlerSummaryShouldBeSetCorrectly(int numberOfFailuresFixer1, int numberOfSuccessesFixer1, int numberOfFailuresFixer2, int numberOfSuccessesFixer2)
        {
            var datetime = DateTime.UtcNow;
            CheckPropertyPostFinish(
                setup: logger => logger
                    .AddFailedFixer(ActivityDiscrepancyType.NotFoundInCosmos, numberOfFailuresFixer1)
                    .AddSuccessFixer(ActivityDiscrepancyType.NotFoundInCosmos, numberOfSuccessesFixer1)
                    .AddFailedFixer2(ActivityDiscrepancyType.NotFoundInCosmos, numberOfFailuresFixer2)
                    .AddSuccessFixer2(ActivityDiscrepancyType.NotFoundInCosmos, numberOfSuccessesFixer2),
                assert: logger =>
                {
                    AssertFixerSummaryIsCorrect<TestFixer>(logger, numberOfFailuresFixer1, numberOfSuccessesFixer1);
                    AssertFixerSummaryIsCorrect<TestFixer2>(logger, numberOfFailuresFixer2, numberOfSuccessesFixer2);
                });
        }

        [Test]
        public void Finish_AfterFailures_ExceptionsShouldBeSetCorrectly()
        {
            const int errorType1 = 1;
            const int errorType2 = 2;
            const int errorType3 = 5;
            const int expectedNumberOfExceptionTypes = 3;

            var datetime = DateTime.UtcNow;
            CheckPropertyPostFinish(
                setup: logger => logger
                    .AddFailedFixer(ActivityDiscrepancyType.NotFoundInCosmos, errorType1, "Error1")
                    .AddFailedFixer(ActivityDiscrepancyType.NotFoundInCosmos, errorType2, "Error2")
                    .AddFailedFixer(ActivityDiscrepancyType.NotFoundInCosmos, errorType3, "Error3"),
                assert: logger =>
                {
                    Assert.AreEqual(expectedNumberOfExceptionTypes, logger.ExceptionSummary.Length, "Exception summary has the wrong number of error types");
                    Assert.AreEqual(errorType1, logger.ExceptionSummary.Single(exsum => exsum.Exception == "Error1").Occurrences, "Wrong number of error 1 recorded");
                    Assert.AreEqual(errorType2, logger.ExceptionSummary.Single(exsum => exsum.Exception == "Error2").Occurrences, "Wrong number of error 2 recorded");
                    Assert.AreEqual(errorType3, logger.ExceptionSummary.Single(exsum => exsum.Exception == "Error3").Occurrences, "Wrong number of error 3 recorded");
                });
        }

        private void CheckPropertyPostFinish(Action<FixActionLogger> assert, Action<FixActionLogger> setup = null)
        {
            var logger = new FixActionLogger();
            logger.Start();
            setup?.Invoke(logger);
            logger.Finish();

            assert(logger);
        }

        private void AssertFixerSummaryIsCorrect<TFixerType>(FixActionLogger logger, int expectedNumberOfFailures, int expectedNumberOfSuccesses) where TFixerType : IActivityDiscrepancyFixer
        {
            var handlerSummary = logger.HandlerSummary.SingleOrDefault(hs => hs.Handler == typeof(TFixerType).FullName);

            if (handlerSummary == null)
            {
                if (expectedNumberOfFailures == 0 && expectedNumberOfSuccesses == 0)
                {
                    return;
                }
                Assert.Fail($"A handler summary for {typeof(TFixerType).Name} has not been created when {expectedNumberOfFailures} failures and {expectedNumberOfSuccesses} successes were expected");
            }

            Assert.AreEqual(expectedNumberOfFailures, handlerSummary.Fail);
            Assert.AreEqual(expectedNumberOfSuccesses, handlerSummary.Success);
            Assert.AreEqual(expectedNumberOfFailures + expectedNumberOfSuccesses, handlerSummary.Occurrences);
            Assert.AreEqual(expectedNumberOfFailures + expectedNumberOfSuccesses, handlerSummary.ExecutionTime);
        }
    }

    static class FixActionLoggerExtensions
    {
        public static FixActionLogger AddFailedFixer(this FixActionLogger logger, ActivityDiscrepancyType discrepancyType, int instances = 1, string error = "Test Error")
        {
            return logger.AddFixer<TestFixer>(discrepancyType, instances, error);
        }

        public static FixActionLogger AddSuccessFixer(this FixActionLogger logger, ActivityDiscrepancyType discrepancyType, int instances = 1)
        {
            return logger.AddFixer<TestFixer>(discrepancyType, instances, null);
        }

        public static FixActionLogger AddFailedFixer2(this FixActionLogger logger, ActivityDiscrepancyType discrepancyType, int instances = 1, string error = "Test Error")
        {
            return logger.AddFixer<TestFixer2>(discrepancyType, instances, error);
        }

        public static FixActionLogger AddSuccessFixer2(this FixActionLogger logger, ActivityDiscrepancyType discrepancyType, int instances = 1)
        {
            return logger.AddFixer<TestFixer2>(discrepancyType, instances, null);
        }

        private static FixActionLogger AddFixer<TFixerType>(this FixActionLogger logger, ActivityDiscrepancyType discrepancyType, int instances, string error) where TFixerType : IActivityDiscrepancyFixer, new()
        {
            for (int i = 0; i < instances; i++)
            {
                var item = CreateLogItem(logger, discrepancyType);

                item.Add(new FixActionHandlerLoggerItem(new TFixerType())
                {
                    Error = error,
                    FixMSecs = 1
                });
            }

            return logger;
        }

        private static FixActionLoggerItem CreateLogItem(FixActionLogger logger, ActivityDiscrepancyType discrepancyType)
        {
            var newItem = new FixActionLoggerItem
            {
                Discrepancy = discrepancyType
            };

            logger.Add(newItem);

            return newItem;
        }
    }

    public class TestFixer : IActivityDiscrepancyFixer
    {
        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return true;
        }

        public Task FixAsync(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class TestFixer2 : IActivityDiscrepancyFixer
    {
        public bool CanHandle(ActivityDiscrepancy discrepancy)
        {
            return true;
        }

        public Task FixAsync(ActivityDiscrepancy discrepancy, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
