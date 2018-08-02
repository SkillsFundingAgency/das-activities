using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using StructureMap;

namespace SFA.DAS.Activities.UnitTests.Client
{
    [TestFixture]
    [Ignore("This connects to real ES and relies on knowledge of specific activities")]
    public class ActivitiesClientTests
    {
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

            var result = await ac.GetLatestActivities(134734);
        }

        [Test]
        public async Task GetLatest_Valid_ShouldReturnTwoRows()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();

            var result = await ac.GetLatestActivities(134734);

            Assert.AreEqual(2, result.Aggregates.Count());
        }

        [Test]
        public async Task GetLatest_Valid_ShouldReturnTotalSetTo3()
        {
            var ioc = GetIoC();
            var ac = ioc.GetInstance<IActivitiesClient>();

            var result = await ac.GetLatestActivities(134734);

            Assert.AreEqual(3, result.Total);
        }

        private Container GetIoC()
        {
            return new Container(c =>
                c.AddRegistry<ActivitiesClientRegistry>());
        }
    }
}
