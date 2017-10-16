using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.Application.Commands.SaveActivity;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.Activities.Application.Validation;
using SFA.DAS.Activities.Worker;

namespace SFA.DAS.Activities.Application.UnitTests.Commands.SaveTaskCommandTests
{
    public class WhenISaveATask : QueryBaseTest<SaveActivityCommandHandler, SaveActivityCommand, SaveActivityCommandResponse>
    {
        private const string OwnerId = "123";

        private Mock<IActivitiesRepository> _repository;

        public override SaveActivityCommand Query { get; set; }
        public override SaveActivityCommandHandler RequestHandler { get; set; }
        public override Mock<IValidator<SaveActivityCommand>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _repository = new Mock<IActivitiesRepository>();

            RequestHandler = new SaveActivityCommandHandler(_repository.Object, RequestValidator.Object);

            Query = new SaveActivityCommand(new Activity().WithOwnerId(OwnerId));

        }
      
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            await RequestHandler.Handle(Query);

            var activityEquivalent = new Activity().WithOwnerId(OwnerId);

            _repository.Verify(x => x.GetActivity(activityEquivalent), Times.Once);
            _repository.Verify(x => x.SaveActivity(It.Is<Activity>(t => t.OwnerId.Equals(Query.Activity.OwnerId))));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            var response = await RequestHandler.Handle(Query);

            Assert.IsNotNull(response);
        }
    }
}
