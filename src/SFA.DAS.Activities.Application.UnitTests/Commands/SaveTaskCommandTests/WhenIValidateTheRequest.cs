//using NuGet;
//using NUnit.Framework;
//using SFA.DAS.Activities.Application.Commands.SaveActivity;

//namespace SFA.DAS.Activities.Application.UnitTests.Commands.SaveTaskCommandTests
//{
//    public class WhenIValidateTheRequest
//    {
//        private SaveActivityRequestValidator _validator;
//        private const long AccountId = 1234;

//        [SetUp]
//        public void Arrange()
//        {
//            _validator = new SaveActivityRequestValidator();
//        }

//        [Test]
//        public void ThenIShouldPassValidationWithAValidRequest()
//        {
//            var request = new SaveActivityCommand(new FluentActivity().AccountId(AccountId).Object());

//            var result = _validator.Validate(request);

//            Assert.IsTrue(result.IsValid());
//        }

//        [Test]
//        public void ThenIShouldFailValidationIfOwnerIdIsNotPresent()
//        {
//            //Arrange

//            var request = new SaveActivityCommand(new FluentActivity().AccountId(0).Object());

//            //Act
//            var result = _validator.Validate(request);

//            //Assert
//            Assert.IsFalse(result.IsValid());
//            Assert.AreEqual("Cannot save Activity when owner ID is not given.", result.ValidationDictionary[nameof(request.Activity.AccountId)]);
//        }
//    }
//}
