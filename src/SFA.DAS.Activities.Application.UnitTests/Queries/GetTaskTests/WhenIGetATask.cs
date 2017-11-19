//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Moq;
//using NuGet;
//using NUnit.Framework;
//using SFA.DAS.Activities.Application.Queries;
//using SFA.DAS.Activities.Application.Queries.GetActivities;
//using SFA.DAS.Activities.Application.Repositories;
//using SFA.DAS.Activities.Application.Validation;


//namespace SFA.DAS.Activities.Application.UnitTests.Queries.GetTaskTests
//{
//    public class WhenIGetATask : QueryBaseTest<GetActivitiesByOwnerIdHandler, GetActivitiesByOwnerIdRequest, GetActivitiesByOwnerIdResponse>
//    {
//        private const long AccountId = 1234;

//        private Mock<IActivitiesRepository> _repository;
//        private Activity _activity;

//        public override GetActivitiesByOwnerIdRequest Query { get; set; }
//        public override GetActivitiesByOwnerIdHandler RequestHandler { get; set; }
//        public override Mock<IValidator<GetActivitiesByOwnerIdRequest>> RequestValidator { get; set; }
     

//        [SetUp]
//        public void Arrange()
//        {

//            SetUp();

//            _activity = new FluentActivity()
//                .AccountId(AccountId)
//                .Object();
//             _repository = new Mock<IActivitiesRepository>();

//            RequestHandler = new GetActivitiesByOwnerIdHandler(_repository.Object, RequestValidator.Object);
//            Query = new GetActivitiesByOwnerIdRequest(AccountId);

//            _repository.Setup(x => x.GetActivities(It.IsAny<long>()))
//                       .ReturnsAsync(new List<Activity>{_activity});
//        }
        
//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCheckedBeforeSaving()
//        {
//            await RequestHandler.Handle(Query);

//            _repository.Verify(x => x.GetActivities(Query.AccountId), Times.Once);
//        }

//        [Test]
//        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
//        {
//            var response = await RequestHandler.Handle(Query);

//            Assert.IsTrue(response.Activities.Contains(_activity));
//        }
//    }
//}
