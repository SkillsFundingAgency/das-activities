using NUnit.Framework;
using SFA.DAS.Activities.Application.Queries.GetActivities;

namespace SFA.DAS.Activities.Application.UnitTests.Queries.GetTasksByOwnerIdTests
{
    public class WhenIValidateTheRequest
    {
        private GetActivitiesByOwnerIdRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetActivitiesByOwnerIdRequestValidator();
        }

        [Test]
        public void ThenIShouldPassValidationWithAValidRequest()
        {
            //Arrange
            var request = new GetActivitiesByOwnerIdRequest("123");

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailValidationIfOwnerIdIsNotPresent()
        {
            //Arrange
            var request = new GetActivitiesByOwnerIdRequest(null);

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual("Cannot get tasks when owner ID is not given.", result.ValidationDictionary[nameof(request.OwnerId)]);
        }
    }
}
