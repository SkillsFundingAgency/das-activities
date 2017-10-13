using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tasks.Application.Queries.GetTasksByOwnerId;
using SFA.DAS.Tasks.Application.Validation;
using SFA.DAS.Tasks.Domain.Repositories;

namespace SFA.DAS.Tasks.Application.UnitTests.Queries.GetTasksByOwnerIdTests
{
    public class WhenIGetTasks : QueryBaseTest<GetTasksByOwnerIdHandler, GetTasksByOwnerIdRequest, GetTasksByOwnerIdResponse>
    {
        private const string TaskOwnerId = "123ACX";

        private Mock<ITaskRepository> _repository;
        private List<Domain.Models.DasTask> _tasks;

        public override GetTasksByOwnerIdRequest Query { get; set; }
        public override GetTasksByOwnerIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTasksByOwnerIdRequest>> RequestValidator { get; set; }
        

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _tasks = new List<Domain.Models.DasTask>
            {
                new Domain.Models.DasTask()
            };

            _repository = new Mock<ITaskRepository>();
            _repository.Setup(x => x.GetTasks(It.IsAny<string>())).ReturnsAsync(_tasks);
            
            RequestHandler = new GetTasksByOwnerIdHandler(_repository.Object, RequestValidator.Object);
            Query = new GetTasksByOwnerIdRequest{ OwnerId = TaskOwnerId};
        }
       
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTasks(TaskOwnerId), Times.Once);
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_tasks, result.Tasks);
        }
    }
}
