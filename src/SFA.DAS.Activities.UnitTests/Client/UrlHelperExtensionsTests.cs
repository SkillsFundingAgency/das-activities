using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.Activities.UnitTests.Client
{
    public static class UrlHelperExtensionsTests
    {
        public class When_getting_activities_url_without_limit : WebTest
        {
            private UrlHelper _urlHelper;
            private string _url;

            protected override void Given()
            {
                _urlHelper = new UrlHelper(ViewContext.RequestContext, Routes);
            }

            protected override void When()
            {
                _url = _urlHelper.Activities();
            }

            [Test]
            public void Then_should_return_correct_url()
            {
                Assert.That(_url, Is.Not.Null);
                Assert.That(_url, Is.EqualTo("/EmployerTeam/Activity"));
            }
        }

        public class When_getting_activities_url_with_limit : WebTest
        {
            private const long Take = 100;

            private UrlHelper _urlHelper;
            private string _url;

            protected override void Given()
            {
                _urlHelper = new UrlHelper(ViewContext.RequestContext, Routes);
            }

            protected override void When()
            {
                _url = _urlHelper.Activities(Take);
            }

            [Test]
            public void Then_should_return_correct_url()
            {
                Assert.That(_url, Is.Not.Null);
                Assert.That(_url, Is.EqualTo("/EmployerTeam/Activity?take=" + Take));
            }
        }

        public class When_getting_activity_url_for_type_with_action : WebTest
        {
            private UrlHelper _urlHelper;
            private string _url;

            private readonly Activity _activity = new Activity
            {
                Type = ActivityType.PayeSchemeAdded
            };

            protected override void Given()
            {
                _urlHelper = new UrlHelper(ViewContext.RequestContext, Routes);
            }

            protected override void When()
            {
                _url = _urlHelper.Activity(_activity);
            }

            [Test]
            public void Then_should_return_correct_url()
            {
                Assert.That(_url, Is.Not.Null);
                Assert.That(_url, Is.EqualTo("/EmployerAccountPaye"));
            }
        }

        public class When_getting_activity_url_for_type_without_action : WebTest
        {
            private UrlHelper _urlHelper;
            private string _url;

            private readonly Activity _activity = new Activity
            {
                Type = ActivityType.Unknown
            };

            protected override void Given()
            {
                _urlHelper = new UrlHelper(ViewContext.RequestContext, Routes);
            }

            protected override void When()
            {
                _url = _urlHelper.Activity(_activity);
            }

            [Test]
            public void Then_should_return_correct_url()
            {
                Assert.That(_url, Is.Null);
            }
        }
    }
}