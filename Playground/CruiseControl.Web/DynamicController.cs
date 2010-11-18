namespace CruiseControl.Web
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using NLog;

    /// <summary>
    /// A controller to allow dynamic resolution of views.
    /// </summary>
    public class DynamicController
        : Controller
    {
        #region Private fields
        private static Logger sLogger = LogManager.GetCurrentClassLogger();
        private static string sDataFolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="DynamicController"/> class.
        /// </summary>
        static DynamicController()
        {
            sDataFolder = ConfigurationStore.DataDirectory;
            sLogger.Debug("Data folder is {0}", sDataFolder);
        }
        #endregion

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
            // Generate the context
            sLogger.Debug("Dynamically resolving request");
            var context = this.GenerateContext(server, project, build, report);

            // Resolve the action handler
            var handler = RetrieveHandler(context.Report);

            // Execute the handler
            ActionResult result;
            if (handler != null)
            {
                sLogger.Debug("Generating action response");
                try
                {
                    result = handler.Value.Generate(context);
                }
                catch (Exception error)
                {
                    sLogger.ErrorException("An error occurring while generating response", error);
                    result = Content("TODO: Display relevant error");
                }
            }
            else
            {
                sLogger.Debug("Generating error message");
                result = Content("TODO: Display relevant error");
            }

            // Return the result
            return result;
        }
        #endregion

        #region RetrieveHandler()
        /// <summary>
        /// Retrieves the handler.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>The details of the handler if found; <c>null</c> otherwise.</returns>
        public Lazy<ActionHandler, IActionHandlerMetadata> RetrieveHandler(string actionName)
        {
            sLogger.Debug("Retrieving action handler for {0}", actionName);
            var handler = ActionHandlerFactory
                .Default
                .ActionHandlers
                .FirstOrDefault(actionHandler => string.Equals(actionHandler.Metadata.Name, actionName, StringComparison.InvariantCultureIgnoreCase));
            sLogger.Info("Unable to find action handler for {0}", actionName);
            return handler;
        }
        #endregion

        #region GenerateContext()
        /// <summary>
        /// Generates the context for an action.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="project">The project.</param>
        /// <param name="build">The build.</param>
        /// <param name="report">The report.</param>
        /// <returns>The <see cref="ActionRequestContext"/> for the action.</returns>
        public ActionRequestContext GenerateContext(string server, string project, string build, string report)
        {
            sLogger.Debug("Generating request context");
            throw new NotImplementedException();
        }
        #endregion

        #region OverrideLogger()
        /// <summary>
        /// Overrides the default logger.
        /// </summary>
        /// <param name="newLogger">The new logger.</param>
        public static void OverrideLogger(Logger newLogger)
        {
            sLogger = newLogger;
        }
        #endregion

        #region ResetLogger()
        /// <summary>
        /// Resets the logger to the default.
        /// </summary>
        public static void ResetLogger()
        {
            sLogger = LogManager.GetCurrentClassLogger();
        }
        #endregion
        #endregion
    }
}
