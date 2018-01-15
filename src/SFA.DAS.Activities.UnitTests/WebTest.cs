using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace SFA.DAS.Activities.UnitTests
{
    public abstract class WebTest : Test
    {
        protected HttpContextBase HttpContextBase { get; private set; }
        protected ViewContext ViewContext { get; private set; }
        protected IViewDataContainer ViewDataContainer { get; private set; }
        protected RouteCollection Routes { get; private set; }

        public override void SetUp()
        {
            var request = new Mock<HttpRequestBase>();

            request.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("/");
            request.SetupGet(r => r.ApplicationPath).Returns("/");
            request.SetupGet(r => r.Url).Returns(new Uri("http://localhost", UriKind.Absolute));
            request.SetupGet(r => r.ServerVariables).Returns(new NameValueCollection());

            var response = new Mock<HttpResponseBase>();

            response.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
            response.SetupProperty(r => r.StatusCode, (int)HttpStatusCode.OK);

            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();

            context.SetupGet(c => c.Request).Returns(request.Object);
            context.SetupGet(c => c.Response).Returns(response.Object);
            context.Setup(c => c.Session).Returns(session.Object);
            context.Setup(c => c.Server).Returns(server.Object);

            var viewData = new ViewDataDictionary();
            var viewDataContainer = new Mock<IViewDataContainer>();

            viewDataContainer.Setup(c => c.ViewData).Returns(viewData);

            var tempData = new TempDataDictionary();
            var routeData = new RouteData();

            var viewContext = new Mock<ViewContext>(
                new ControllerContext(context.Object, routeData, new Mock<ControllerBase>().Object),
                new Mock<IView>().Object,
                viewData,
                tempData,
                new StreamWriter(new MemoryStream()));

            var routes = new RouteCollection();

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            HttpContextBase = context.Object;
            ViewDataContainer = viewDataContainer.Object;
            ViewContext = viewContext.Object;
            Routes = routes;

            base.SetUp();
        }
    }
}