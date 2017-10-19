using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NuGet;
using NUnit.Framework;
using SFA.DAS.Activities.Application.Queries.GetActivities;
using SFA.DAS.Activities.Application.Repositories;
using SFA.DAS.Activities.Application.Validation;

namespace SFA.DAS.Activities.Application.UnitTests.Queries.GetTasksByOwnerIdTests
{
    public class WhenIGetTasks : QueryBaseTest<GetActivitiesByOwnerIdHandler, GetActivitiesByOwnerIdRequest, GetActivitiesByOwnerIdResponse>
    {
        private const string TaskOwnerId = "123ACX";

        private Mock<IActivitiesRepository> _repository;
        private List<Activity> _activities;

        public override GetActivitiesByOwnerIdRequest Query { get; set; }
        public override GetActivitiesByOwnerIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetActivitiesByOwnerIdRequest>> RequestValidator { get; set; }
        

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _activities = new List<Activity>
            {
                new Activity()
            };

            _repository = new Mock<IActivitiesRepository>();
            _repository.Setup(x => x.GetActivities(It.IsAny<string>())).ReturnsAsync(_activities);
            
            RequestHandler = new GetActivitiesByOwnerIdHandler(_repository.Object, RequestValidator.Object);
            Query = new GetActivitiesByOwnerIdRequest(TaskOwnerId);
        }
       
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCheckedBeforeSaving()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetActivities(TaskOwnerId), Times.Once);
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_activities, result.Activities);
        }
    }
}
