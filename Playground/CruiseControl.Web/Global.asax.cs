namespace CruiseControl.Web
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The main application start-up and shut-down.
    /// </summary>
    public class MvcApplication 
        : HttpApplication
    {
        #region Public methods
        #region RegisterRoutes()
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The <see cref="System.Web.Routing.RouteCollection"/> to register the routes in.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Default",
                "{server}/{project}/{build}/{report}",
                new
                    {
                        controller = "Dynamic",
                        action = "Index",
                        server = string.Empty,
                        project = string.Empty,
                        build = string.Empty,
                        report = string.Empty
                    });

        }
        #endregion
        #endregion

        #region Protected methods
        #region Application_Start()
        /// <summary>
        /// Starts the application.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
        #endregion
        #endregion
    }
}