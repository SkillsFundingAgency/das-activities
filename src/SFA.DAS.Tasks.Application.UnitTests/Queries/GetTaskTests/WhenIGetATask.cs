using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tasks.Application.Queries.GetTask;
using SFA.DAS.Tasks.Application.Validation;
using SFA.DAS.Tasks.API.Types.Enums;
using SFA.DAS.Tasks.Domain.Models;
using SFA.DAS.Tasks.Domain.Repositories;

namespace SFA.DAS.Tasks.Application.UnitTests.Queries.GetTaskTests
{
    public class WhenIGetATask : QueryBaseTest<GetTaskRequestHandler, GetTaskRequest, GetTaskResponse>
    {
        private Mock<ITaskRepository> _repository;
        private DasTask _task;

        public override GetTaskRequest Query { get; set; }
        public override GetTaskRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetTaskRequest>> RequestValidator { get; set; }
     

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _task = new DasTask();
            _repository = new Mock<ITaskRepository>();

            RequestHandler = new GetTaskRequestHandler(_repository.Object, RequestValidator.Object);
            Query = new GetTaskRequest
            {
                OwnerId = "123",
                Type = TaskType.AgreementToSign
            };

            _repository.Setup(x => x.GetTask(It.IsAny<string>(), It.IsAny<TaskType>()))
                       .ReturnsAsync(_task);
        }
        
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTask(Query.OwnerId, Query.Type), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_task, response.Task);
        }
    }
}
