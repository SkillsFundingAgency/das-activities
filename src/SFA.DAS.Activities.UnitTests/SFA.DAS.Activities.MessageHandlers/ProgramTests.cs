using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.MessageHandlers;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.Activities.UnitTests.SFA.DAS.Activities.MessageHandlers
{
    [TestFixture]
    public class ProgramTests
    {
        [Test, Description("When there are no message processors then we should exit early")]
        public void StartMessageProcessors_NoHandlers_ShouldStopWithoutWaitingForCancellation()
        {
            var fixtures = new ProgramTestFixtures();

            fixtures
                .StartMessageProcessors()
                .WaitForMessageProcessorsToStop(ProgramTestFixtures.MaxTimeBeforeAbandonTest, isStopped => Assert.IsTrue(isStopped));
        }

        [Test, Description("When webjob does not cancel and no messages processors throw an ex then we should run forever")]
        public void StartMessageProcessors_SingleHandlerWithoutExceptionAndNoCancellation_ShouldNotStop()
        {
            var fixtures = new ProgramTestFixtures();

            fixtures
                .AddProcessor()
                .StartMessageProcessors()
                .WaitForMessageProcessorsToStop(ProgramTestFixtures.MaxTimeBeforeAbandonTest, isStopped => Assert.IsFalse(isStopped));
        }

        [Test, Description("When a single processor runs without exception then it should run until canceled by web job")]
        public void StartMessageProcessors_SingleHandlerWithoutException_ShouldStopWhenCancelled()
        {
            var fixtures = new ProgramTestFixtures();

            fixtures
                .AddProcessor()
                .StartMessageProcessors(5)
                .WaitForMessageProcessorsToStop(ProgramTestFixtures.MaxTimeBeforeAbandonTest, isStopped => Assert.IsTrue(isStopped));
        }

        [Test, Description("When a single processor runs and throws an exception then job should continue")]
        public void StartMessageProcessors_SingleHandlerWithException_ShouldStopWithoutCancellation()
        {
            var fixtures = new ProgramTestFixtures();

            fixtures
                .AddProcessor()
                .WithExcetionDuringProcessing()
                .StartMessageProcessors()
                .WaitForMessageProcessorsToStop(ProgramTestFixtures.MaxTimeBeforeAbandonTest, isStopped => Assert.IsFalse(isStopped));
        }

        [Test]
        public void StartMessageProcessors_MultipleHandlersOneWithException_ShouldRunForever()
        {
            var fixtures = new ProgramTestFixtures();

            fixtures
                .AddProcessor()
                .AddProcessor()
                .AddProcessor()
                .WithExcetionDuringProcessing()
                .StartMessageProcessors()
                .WaitForMessageProcessorsToStop(ProgramTestFixtures.MaxTimeBeforeAbandonTest, isStopped => Assert.IsFalse(isStopped));
        }
    }

    public class ProgramTestFixtures
    {
        private readonly List<TestMessageProcessor> _messageProcessors = new List<TestMessageProcessor>();

        public ProgramTestFixtures()
        {
            TextWriter = new StringWriter();
            CancellationTokenSource = new CancellationTokenSource();

            ContainerMock = new Mock<IContainer>();

            ContainerMock
                .Setup(cm => cm.GetAllInstances<IMessageProcessor2>())
                .Returns(() => _messageProcessors);
        }

        public Mock<IContainer> ContainerMock { get; }
        public IContainer Container => ContainerMock.Object;
        public TestMessageProcessor CurrentMessageProcessor { get; private set; }

        public const int MaxTimeBeforeAbandonTest = 5000;



        public ProgramTestFixtures AddProcessor()
        {
            var messageProcessor = new TestMessageProcessor(5000);

            _messageProcessors.Add(messageProcessor);

            CurrentMessageProcessor = messageProcessor;
            return this;
        }

        public ProgramTestFixtures WithExcetionDuringProcessing()
        {
            CurrentMessageProcessor.ThrowExceptionDuringProcessing = true;
            return this;
        }

        public TextWriter TextWriter { get; }

        public CancellationTokenSource CancellationTokenSource { get; }

        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        public bool IsRunning => Program.MessageProcessorRunning.IsSet;

        public bool AllMessageProcessorsHaveStopped
        {
            get
            {
                return WaitHandle.WaitAll(_messageProcessors.Select(mp => mp.HasStopped.WaitHandle).ToArray(), MaxTimeBeforeAbandonTest);
            }
        }

        public ProgramTestFixtures WaitForMessageProcessorsToStop(int maxWaitTimeMSecs, Action<bool> afterWaitAction)
        {
            var isStopped = Program.MessageProcessorStopped.Wait(maxWaitTimeMSecs);

            afterWaitAction?.Invoke(isStopped);

            return this;
        }

        public ProgramTestFixtures StartMessageProcessors()
        {
            return StartMessageProcessors(-1);
        }

        public ProgramTestFixtures StartMessageProcessors(int cancelAfterMSecs)
        {
            ServiceLocator.Initialise(Container);

            Task.Run(() =>
            {
                Program.StartMessageProcessors(TextWriter, CancellationToken);
            });

            if (cancelAfterMSecs > -1)
            {
                CancellationTokenSource.CancelAfter(cancelAfterMSecs);
            }

            Program.MessageProcessorRunning.Wait(30);

            return this;
        }
    }

    public class TestMessageProcessingException : Exception
    {

    }

    public class TestMessageProcessor : IMessageProcessor2
    {
        private readonly int _maxwaitForCancelationMSecs;

        public TestMessageProcessor(int maxwaitForCancelationMSecs)
        {
            _maxwaitForCancelationMSecs = maxwaitForCancelationMSecs;
        }

        public bool ThrowExceptionDuringProcessing { get; set; }

        public ManualResetEventSlim HasStopped { get; } = new ManualResetEventSlim(false);

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (ThrowExceptionDuringProcessing)
                {
                    throw new TestMessageProcessingException();
                }

                cancellationToken.WaitHandle.WaitOne(_maxwaitForCancelationMSecs);
            }).ContinueWith(t =>
            {
                HasStopped.Set();
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }
            });
        }
    }
}
