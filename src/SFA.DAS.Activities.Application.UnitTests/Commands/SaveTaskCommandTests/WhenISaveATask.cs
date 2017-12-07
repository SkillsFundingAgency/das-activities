//using System;
//using System.Threading.Tasks;
//using Moq;
//using NuGet;
//using NUnit.Framework;
//using SFA.DAS.Activities.Application.Commands.SaveActivity;
//using SFA.DAS.Activities.Application.Repositories;
//using SFA.DAS.Activities.Application.Validation;

//namespace SFA.DAS.Activities.Application.UnitTests.Commands.SaveTaskCommandTests
//{
//    public class WhenISaveATask : QueryBaseTest<SaveActivityCommandHandler, SaveActivityCommand, SaveActivityCommandResponse>
//    {
//        private const long AccountId = 1234;

//        private Mock<IActivitiesRepository> _repository;

//        private Activity _testActivity;

//        public override SaveActivityCommand Query { get; set; }
//        public override SaveActivityCommandHandler RequestHandler { get; set; }
//        //public override Mock<IValidator<SaveActivityCommand>> RequestValidator { get; set; }

//        [SetUp]
//        public void Arrange()
//        {
//            base.SetUp();

//            _repository = new Mock<IActivitiesRepository>();

//            RequestHandler = new SaveActivityCommandHandler(_repository.Object);

//            _testActivity = new FluentActivity().AccountId(AccountId).Object();

//            Query = new SaveActivityCommand(_testActivity);
//        }
      
//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCheckedBeforeSaving()
//        {
//            await RequestHandler.Handle(Query);

//            _repository.Verify(x => x.SaveActivity(_testActivity));
//        }

//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
//        {
//            var response = await RequestHandler.Handle(Query);

//            Assert.IsNotNull(response);
//        }
//    }
//}
