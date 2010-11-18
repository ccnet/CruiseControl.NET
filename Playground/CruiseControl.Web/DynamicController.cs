namespace CruiseControl.Web
{
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// A controller to allow dynamic resolution of views.
    /// </summary>
    public class DynamicController 
        : Controller
    {
        #region Public methods
        #region Index()
        /// <summary>
        /// Dynamically resolves a view.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="project">The project.</param>
        /// <param name="build">The build.</param>
        /// <param name="report">The report.</param>
        /// <returns>The resolved view.</returns>
        public ActionResult Index(string server, string project, string build, string report)
        {
            // Resolve the action name
            var actionName = report;

            // Resolve the action handler
            var handler = ActionHandlerFactory
                .Default
                .ActionHandlers
                .FirstOrDefault(actionHandler => actionHandler.Metadata.Name == actionName);

            // Execute the handler
            ActionResult result;
            if (handler != null)
            {
                result = handler.Value.Execute();
            }
            else
            {
                result = Content("TODO: Display relevant error");
            }

            // Return the result
            return result;
        }
        #endregion
        #endregion
    }
}
