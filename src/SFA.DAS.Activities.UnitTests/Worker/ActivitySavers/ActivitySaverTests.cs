using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.Worker.ActivitySavers;
using SFA.DAS.Activities.Worker.ObjectMappers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Activities.UnitTests.Worker.ActivitySavers
{
    [TestFixture]
    public class ActivitySaverTests
    {
        [Test]
        public void Constructor_MissingActivity_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>( () => new ActivitySaver(null, CosmosClient, ElasticClient, Logger, MessageContextProvider, MessageServiceBusConfiguration));
        }

        [Test]
        public void Constructor_MissingCosmosClient_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, null, ElasticClient, Logger, MessageContextProvider, MessageServiceBusConfiguration));
        }

        [Test]
        public void Constructor_MissingElasticClient_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosClient, null, Logger, MessageContextProvider, MessageServiceBusConfiguration));
        }

        [Test]
        public void Constructor_MissingLogger_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosClient, ElasticClient, null, MessageContextProvider, MessageServiceBusConfiguration));
        }

        [Test]
        public void Constructor_MissingMessageContextProvider_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosClient, ElasticClient, Logger, null, MessageServiceBusConfiguration));
        }

        [Test]
        public void Constructor_MissingMessageServiceBusConfiguration_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosClient, ElasticClient, Logger, MessageContextProvider, null));
        }

        public Mock<IActivityMapper> ActivityMapperMock = new Mock<IActivityMapper>();
        public IActivityMapper ActivityMapper => ActivityMapperMock.Object;

        public Mock<ICosmosClient> CosmosClientMock = new Mock<ICosmosClient>();
        public ICosmosClient CosmosClient => CosmosClientMock.Object;

        public Mock<IElasticClient> ElasticClientMock = new Mock<IElasticClient>();
        public IElasticClient ElasticClient => ElasticClientMock.Object;

        public Mock<ILog> LoggerMock = new Mock<ILog>();
        public ILog Logger => LoggerMock.Object;

        public Mock<IMessageContextProvider> MessageContextProviderMock = new Mock<IMessageContextProvider>();
        public IMessageContextProvider MessageContextProvider => MessageContextProviderMock.Object;

        public Mock<IMessageServiceBusConfiguration> MessageServiceBusConfigurationMock = new Mock<IMessageServiceBusConfiguration>();
        public IMessageServiceBusConfiguration MessageServiceBusConfiguration => MessageServiceBusConfigurationMock.Object;
    }
}
