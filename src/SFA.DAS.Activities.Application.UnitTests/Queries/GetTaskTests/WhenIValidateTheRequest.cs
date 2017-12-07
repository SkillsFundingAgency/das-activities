using NuGet;
using NUnit.Framework;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.Application.Queries.GetActivities;

namespace SFA.DAS.Activities.Application.UnitTests.Queries.GetTaskTests
{
    public class WhenIValidateTheRequest
    {
        private SaveActivityRequestValidator _validator;
        private const long Account = 1234;

        [SetUp]
        public void Arrange()
        {
            _validator = new SaveActivityRequestValidator();
        }

        [Test]
        public void ThenIShouldPassValidationWithAValidRequest()
        {
            //Arrange
            var request = new SaveActivityCommand(new Activity {AccountId = Account});

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldFailValidationIfOwnerIdIsNotPresent()
        {
            //Arrange
            var request = new SaveActivityCommand(new Activity());

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual("Cannot save Activity when hashed ID is not given.", result.ValidationDictionary[nameof(request.Activity.AccountId)]);
        }
    }
}
