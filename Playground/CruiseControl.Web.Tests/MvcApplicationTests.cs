namespace CruiseControl.Web.Tests
{
    using System.Web.Routing;
    using MvcContrib.TestHelper;
    using NUnit.Framework;

    [TestFixture]
    public class MvcApplicationTests
    {
        #region Tests
        [Test(Description = "Should handle a default route (i.e. nothing in the query string beyond the site.)")]
        public void DefaultRouteIsHandled()
        {
            var routes = RouteTable.Routes;
            routes.Clear();
            MvcApplication.RegisterRoutes(routes);
            "~/".ShouldMapTo<DynamicController>(action => action.Index(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        [Test(Description = "Should handle a route that just consists of a server.")]
        public void ServerRouteIsHandled()
        {
            var routes = RouteTable.Routes;
            routes.Clear();
            MvcApplication.RegisterRoutes(routes);
            "~/local".ShouldMapTo<DynamicController>(action => action.Index("local", string.Empty, string.Empty, string.Empty));
        }

        [Test(Description = "Should handle a route that consists of a server and a project.")]
        public void ProjectRouteIsHandled()
        {
            var routes = RouteTable.Routes;
            routes.Clear();
            MvcApplication.RegisterRoutes(routes);
            "~/buildServer/aProject".ShouldMapTo<DynamicController>(action => action.Index("buildServer", "aProject", string.Empty, string.Empty));
        }

        [Test(Description = "Should handle a route that consists of a server, a project and a build.")]
        public void BuildRouteIsHandled()
        {
            var routes = RouteTable.Routes;
            routes.Clear();
            MvcApplication.RegisterRoutes(routes);
            "~/server/project/build".ShouldMapTo<DynamicController>(action => action.Index("server", "project", "build", string.Empty));
        }

        [Test(Description = "Should handle a route that consists of all parts.")]
        public void ReportRouteIsHandled()
        {
            var routes = RouteTable.Routes;
            routes.Clear();
            MvcApplication.RegisterRoutes(routes);
            "~/server/project/build/report".ShouldMapTo<DynamicController>(action => action.Index("server", "project", "build", "report"));
        }
        #endregion
    }
}
