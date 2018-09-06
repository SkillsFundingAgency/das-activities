using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.IntegrityChecker.Dto;
using SFA.DAS.Activities.IntegrityChecker.Fixers;
using SFA.DAS.Activities.IntegrityChecker.Interfaces;

namespace SFA.DAS.Activities.UnitTests.IntegrityChecker.Fixers
{
    [TestFixture]
    public class NotFoundInElasticFixerTests
    {
        [TestCase(ActivityDiscrepancyType.None, false)]
        [TestCase(ActivityDiscrepancyType.NotFoundInElastic, true)]
        [TestCase(ActivityDiscrepancyType.NotFoundInCosmos, false)]
        [TestCase(ActivityDiscrepancyType.NotFoundInCosmos | ActivityDiscrepancyType.NotFoundInElastic, true)]
        public void CanHandle_NotFoundInElastic_ShouldReturnTrue(ActivityDiscrepancyType whenCalledWith, bool expectedResult)
        {
            // arrange
            var repo = new Mock<IElasticActivityDocumentRepository>();
            var fixer = new NotFoundInElasticFixer(repo.Object);
            var dto = new ActivityDiscrepancy(new Activity(), whenCalledWith);

            // act
            var actualResult = fixer.CanHandle(dto);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task FixAsync_WhenFixingActivity_ShouldSaveToElastic()
        {
            // arrange
            var repo = new Mock<IElasticActivityDocumentRepository>();
            var fixer = new NotFoundInElasticFixer(repo.Object);
            var dto = new ActivityDiscrepancy(new Activity(), ActivityDiscrepancyType.NotFoundInElastic);

            // act
            await fixer.FixAsync(dto, CancellationToken.None);

            // assert
            repo.Verify(r => r.UpsertActivityAsync(dto.Activity), Times.Once);
        }
    }
}
