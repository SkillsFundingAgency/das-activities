using System;
using NUnit.Framework;
using SFA.DAS.Activities.Extensions;

namespace SFA.DAS.Activities.UnitTests.Extensions
{
    [TestFixture]
    public class ActivityExtensionTests
    {
        [TestCase(ActivityType.PayeSchemeAdded, "EmployerAccountPaye")]
        [TestCase(ActivityType.UserJoined, "EmployerTeam")]
        [TestCase(ActivityType.LegalEntityRemoved, "EmployerAgreement")]
        [TestCase(ActivityType.LegalEntityAdded, "EmployerAgreement")]
        [TestCase(ActivityType.AccountNameChanged, "EmployerTeam")]
        [TestCase(ActivityType.UserInvited, "EmployerTeam")]
        [TestCase(ActivityType.AgreementSigned, "EmployerAgreement")]
        [TestCase(ActivityType.PaymentCreated, "EmployerAccountTransactions")]
        public void GetDetailsLink_ShouldReturnCorrectController(ActivityType type, string expectedController)
        {
            //Assign
            var activity = new Activity{Type = type};

            //Act
            var link = activity.GetDetailsLink();

            //Assert
            Assert.AreEqual(expectedController, link.Controller);
        }


        [TestCase(ActivityType.PayeSchemeAdded, "Index")]
        [TestCase(ActivityType.UserJoined, "ViewTeam")]
        [TestCase(ActivityType.LegalEntityRemoved, "Index")]
        [TestCase(ActivityType.LegalEntityAdded, "Index")]
        [TestCase(ActivityType.AccountNameChanged, "Index")]
        [TestCase(ActivityType.UserInvited, "ViewTeam")]
        [TestCase(ActivityType.AgreementSigned, "Index")]
        [TestCase(ActivityType.PaymentCreated, "Index")]
        public void GetDetailsLink_ShouldReturnCorrectAction(ActivityType type, string expectedAction)
        {
            //Assign
            var activity = new Activity{Type = type};

            //Act
            var link = activity.GetDetailsLink();

            //Assert
            Assert.AreEqual(expectedAction, link.Action);
        }

        [Test]
        public void GetDetailsLink_ShouldThrowExceptionIfLinkUnsupported()
        {
            //Assign
            var activity = new Activity
            {
                Type = ActivityType.Unknown,
                At = DateTime.Now.AddMonths(-2)
            };

            //Act
            var link = activity.GetDetailsLink();

            //Assert
            Assert.IsNull(link);
        }

        [Test]
        public void GetDetailsLink_ShouldReturnNoActionParametersIfUnsupported()
        {
            //Assign
            var activity = new Activity
            {
                Type = ActivityType.UserJoined,
                At = DateTime.Now.AddMonths(-2)
            };

            //Act
            var link = activity.GetDetailsLink();

            //Assert
            Assert.IsEmpty(link.Parameters);
        }

        [Test]
        public void GetDetailsLink_ShouldReturnCorrectPaymentActionParameters()
        {
            //Assign
            var activity = new Activity
            {
                Type = ActivityType.PaymentCreated,
                At = DateTime.Now.AddMonths(-2)
            };

            //Act
            var link = activity.GetDetailsLink();

            //Assert
            Assert.True(link.Parameters.ContainsKey("month"));
            Assert.AreEqual(activity.At.Month, (int) link.Parameters["month"]);
            
            Assert.True(link.Parameters.ContainsKey("year"));
            Assert.AreEqual(activity.At.Year, (int) link.Parameters["year"]);
        }

    }
}
