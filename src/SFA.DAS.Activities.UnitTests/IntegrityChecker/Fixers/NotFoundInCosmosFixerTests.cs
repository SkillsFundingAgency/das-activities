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
    public class NotFoundInCosmosFixerTests
    {
        [TestCase(ActivityDiscrepancyType.None, false)]
        [TestCase(ActivityDiscrepancyType.NotFoundInElastic, false)]
        [TestCase(ActivityDiscrepancyType.NotFoundInCosmos, true)]
        [TestCase(ActivityDiscrepancyType.NotFoundInCosmos | ActivityDiscrepancyType.NotFoundInElastic, true)]
        public void CanHandle_NotFoundInCosmos_ShouldReturnTrue(ActivityDiscrepancyType whenCalledWith, bool expectedResult)
        {
            // arrange
            var repo = new Mock<ICosmosActivityDocumentRepository>();
            var fixer = new NotFoundInCosmosFixer(repo.Object);
            var dto = new ActivityDiscrepancy(new Activity(), whenCalledWith);

            // act
            var actualResult = fixer.CanHandle(dto);

            // assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task FixAsync_WhenFixingActivity_ShouldSaveToCosmos()
        {
            // arrange
            var repo = new Mock<ICosmosActivityDocumentRepository>();
            var fixer = new NotFoundInCosmosFixer(repo.Object);
            var dto = new ActivityDiscrepancy(new Activity(), ActivityDiscrepancyType.NotFoundInCosmos);

            // act
            await fixer.FixAsync(dto, CancellationToken.None);

            // assert
            repo.Verify(r => r.UpsertActivityAsync(dto.Activity), Times.Once);
        }
    }
}
