using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;
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
            Assert.Throws<ArgumentException>( () => new ActivitySaver(null, CosmosActivityDocumentRepository, ElasticActivityDocumentRepository, Logger, MessageContextProvider));
        }

        [Test]
        public void Constructor_MissingCosmosClient_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, null, ElasticActivityDocumentRepository, Logger, MessageContextProvider));
        }

        [Test]
        public void Constructor_MissingElasticClient_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosActivityDocumentRepository, null, Logger, MessageContextProvider));
        }

        [Test]
        public void Constructor_MissingLogger_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosActivityDocumentRepository, ElasticActivityDocumentRepository, null, MessageContextProvider));
        }

        [Test]
        public void Constructor_MissingMessageContextProvider_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new ActivitySaver(ActivityMapper, CosmosActivityDocumentRepository, ElasticActivityDocumentRepository, Logger, null));
        }

        public Mock<IActivityMapper> ActivityMapperMock = new Mock<IActivityMapper>();
        public IActivityMapper ActivityMapper => ActivityMapperMock.Object;

        public Mock<ICosmosActivityDocumentRepository> CosmosActivityRepositoryMock = new Mock<ICosmosActivityDocumentRepository>();
        public ICosmosActivityDocumentRepository CosmosActivityDocumentRepository => CosmosActivityRepositoryMock.Object;

	    public Mock<IElasticActivityDocumentRepository> ElasticActivityRepositoryMock = new Mock<IElasticActivityDocumentRepository>();
	    public IElasticActivityDocumentRepository ElasticActivityDocumentRepository => ElasticActivityRepositoryMock.Object;

		public Mock<ILog> LoggerMock = new Mock<ILog>();
        public ILog Logger => LoggerMock.Object;

        public Mock<IMessageContextProvider> MessageContextProviderMock = new Mock<IMessageContextProvider>();
        public IMessageContextProvider MessageContextProvider => MessageContextProviderMock.Object;
    }
}
