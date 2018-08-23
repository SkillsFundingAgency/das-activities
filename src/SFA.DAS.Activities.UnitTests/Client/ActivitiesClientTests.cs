using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.Activities.IndexMappers;
using SFA.DAS.Elastic;
using StructureMap;

namespace SFA.DAS.Activities.UnitTests.Client
{
    [TestFixture]
    [Ignore("This connects to real ES and relies on knowledge of specific activities")]
    public class ActivitiesClientTests
    {
        private const long TestAccountId = 9650;

        [Test]
        public void Constructor_Valid_ShouldNotThrowException()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();
        }

        [Test]
        public async Task GetLatest_Valid_ShouldReturnActivities()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();

            var result = await ac.GetLatestActivities(TestAccountId);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetLatest_Valid_ShouldReturnTwoRows()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();

            var result = await ac.GetLatestActivities(TestAccountId);

            Assert.AreEqual(2, result.Aggregates.Count());
        }

        [Test]
        public async Task GetLatest_Valid_ShouldReturnTotalSetTo3()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();

            var result = await ac.GetLatestActivities(TestAccountId);

            Assert.AreEqual(3, result.Total);
        }

        private Container GetIoC()
        {
            var ioc = new Container(c =>
                c.AddRegistry<ActivitiesClientRegistry>());

            // If you want to run locally against a test_activities then uncomment the following code:
            //ElasticConfiguration ec = ioc.GetInstance<ElasticConfiguration>();
            //ec.OverrideEnvironmentName("test");
            //ec.ScanForIndexMappers(typeof(ActivitiesIndexMapper).Assembly);

            return ioc;
        }
    }
}