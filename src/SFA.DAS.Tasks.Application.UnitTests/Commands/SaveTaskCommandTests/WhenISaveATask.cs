using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tasks.Application.Commands.SaveTask;
using SFA.DAS.Tasks.Application.Validation;
using SFA.DAS.Tasks.API.Types.Enums;
using SFA.DAS.Tasks.Domain.Models;
using SFA.DAS.Tasks.Domain.Repositories;

namespace SFA.DAS.Tasks.Application.UnitTests.Commands.SaveTaskCommandTests
{
    public class WhenISaveATask : QueryBaseTest<SaveTaskCommandHandler, SaveTaskCommand, SaveTaskCommandResponse>
    {
        private Mock<ITaskRepository> _repository;

        public override SaveTaskCommand Query { get; set; }
        public override SaveTaskCommandHandler RequestHandler { get; set; }
        public override Mock<IValidator<SaveTaskCommand>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _repository = new Mock<ITaskRepository>();

            RequestHandler = new SaveTaskCommandHandler(_repository.Object, RequestValidator.Object);
            Query = new SaveTaskCommand
            {
                OwnerId = "123",
                TaskCompleted = false,
                Type = TaskType.AddApprentices
            };
        }
      
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTask(Query.OwnerId, Query.Type), Times.Once);
            _repository.Verify(x => x.SaveTask(It.Is<DasTask>(t => t.OwnerId.Equals(Query.OwnerId) &&
                                                              t.Type.Equals(Query.Type) &&
                                                              t.ItemsDueCount.Equals(1))), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(response);
        }

        [Test]
        public async Task ThenIfATaskIsAlreadySavedIShouldUpdateItsItemsDueCounter()
        {
            //We represent multiple tasks of the same type and owner as a counter rather than multiple tasks entries

            //Arrange
            var existingTask = new DasTask
            {
                Id = Guid.NewGuid(),
                OwnerId = "123",
                Type = TaskType.AgreementToSign,
                ItemsDueCount = 3
            };

            var expectedItemsDueCount = (ushort)(existingTask.ItemsDueCount + 1);

            _repository.Setup(x => x.GetTask(Query.OwnerId, Query.Type)).ReturnsAsync(existingTask);
            
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTask(Query.OwnerId, Query.Type), Times.Once);
            _repository.Verify(x => x.SaveTask(It.Is<DasTask>(t => t.Id.Equals(existingTask.Id) &&
                                                                   t.OwnerId.Equals(existingTask.OwnerId) &&
                                                                   t.Type.Equals(existingTask.Type) &&
                                                                   t.ItemsDueCount.Equals(expectedItemsDueCount))), Times.Once);
        }

        [Test]
        public async Task ThenIfATaskIsCompletedTheItemsDueCounterShouldBeDecremented()
        {
            //We represent multiple tasks of the same type and owner as a counter rather than multiple tasks entries

            //Arrange
            Query.TaskCompleted = true;

            var existingTask = new DasTask
            {
                Id = Guid.NewGuid(),
                OwnerId = "123",
                Type = TaskType.AgreementToSign,
                ItemsDueCount = 3
            };

            var expectedItemsDueCount = (ushort) (existingTask.ItemsDueCount - 1);

            _repository.Setup(x => x.GetTask(Query.OwnerId, Query.Type)).ReturnsAsync(existingTask);

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTask(Query.OwnerId, Query.Type), Times.Once);
            _repository.Verify(x => x.SaveTask(It.Is<DasTask>(t => t.Id.Equals(existingTask.Id) &&
                                                                   t.OwnerId.Equals(existingTask.OwnerId) &&
                                                                   t.Type.Equals(existingTask.Type) &&
                                                                   t.ItemsDueCount.Equals(expectedItemsDueCount))), Times.Once);
        }

        [Test]
        public async Task ThenDoNotSaveTheTaskIfItIsCompletedAndNoTasksAreCurrentlyStored()
        {
            //We represent multiple tasks of the same type and owner as a counter rather than multiple tasks entries

            //Arrange
            Query.TaskCompleted = true;

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetTask(Query.OwnerId, Query.Type), Times.Once);
            _repository.Verify(x => x.SaveTask(It.IsAny<DasTask>()), Times.Never);
        }
    }
}
