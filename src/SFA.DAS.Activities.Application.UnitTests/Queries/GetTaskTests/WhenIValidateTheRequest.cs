using NUnit.Framework;

namespace SFA.DAS.Activities.Application.UnitTests.Queries.GetTaskTests
{
    public class WhenIValidateTheRequest
    {
        private GetTaskRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetTaskRequestValidator();
        }

        [Test]
        public void ThenIShouldPassValidationWithAValidRequest()
        {
            //Arrange
            var request = new GetTaskRequest {OwnerId = "1233", Type = TaskType.AddApprentices };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailValidationIfOwnerIdIsNotPresent()
        {
            //Arrange
            var request = new GetTaskRequest { Type = TaskType.AddApprentices };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual("Cannot get task when owner ID is not given.", result.ValidationDictionary[nameof(request.OwnerId)]);
        }

        [Test]
        public void ThenIShouldFailValidationIfTaskTypeIsNotPresent()
        {
            //Arrange
            var request = new GetTaskRequest { OwnerId = "1233" }; 

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual("Cannot get task when type is not given.", result.ValidationDictionary[nameof(request.Type)]);
        }
    }
}
