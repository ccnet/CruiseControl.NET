namespace CruiseControl.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Configuration;
    using NLog;

    /// <summary>
    /// A controller to allow dynamic resolution of views.
    /// </summary>
    public class DynamicController
        : Controller
    {
        #region Private fields
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string DataFolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="DynamicController"/> class.
        /// </summary>
        static DynamicController()
        {
            DataFolder = Folders.DataDirectory;
            Logger.Debug("Data folder is {0}", DataFolder);
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
            Logger.Debug("Dynamically resolving request");
            var context = this.GenerateContext(server, project, build, report);

            // Resolve the action handler
            var handler = RetrieveHandler(context.Report);

            // Execute the handler
            ActionResult result;
            if (handler != null)
            {
                Logger.Debug("Generating action response");
                try
                {
                    result = handler.Value.Generate(context);
                }
                catch (Exception error)
                {
                    Logger.ErrorException("An error occurring while generating response", error);
                    result = Content("TODO: Display relevant error");
                }
            }
            else
            {
                Logger.Debug("Generating error message");
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
            Logger.Debug("Retrieving action handler for {0}", actionName);
            var handler = ActionHandlerFactory
                .Default
                .ActionHandlers
                .FirstOrDefault(actionHandler => string.Equals(actionHandler.Metadata.Name, actionName, StringComparison.InvariantCultureIgnoreCase));
            if (handler == null)
            {
                Logger.Info("Unable to find action handler for {0}", actionName);
            }

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
            Logger.Debug("Generating request context");

            // Set the properties to what is received first
            var context = new ActionRequestContext
                              {
                                  Server = string.IsNullOrWhiteSpace(server) ? null : server,
                                  Project = string.IsNullOrWhiteSpace(project) ? null : project,
                                  Build = string.IsNullOrWhiteSpace(build) ? null : build,
                                  Report = string.IsNullOrWhiteSpace(report) ? null : report
                              };
            
            // If everything is set, we are golden - just return the instance
            if ((context.Server != null) &&
                (context.Project != null) &&
                (context.Build != null) &&
                (context.Report != null))
            {
                Logger.Debug("Action is a build level report");
                context.Level = ActionHandlerTargets.Build;
                return context;
            }

            // If nothing is set, find the default root-level action and return that
            if (!CheckLevelForAction(context, ActionHandlerTargets.Root, context.Server, () => context.Server = null))
            {
                if (!CheckLevelForAction(context, ActionHandlerTargets.Server, context.Project, () => context.Project = null))
                {
                    if (!CheckLevelForAction(context, ActionHandlerTargets.Project, context.Build, () => context.Build = null))
                    {
                        CheckLevelForAction(context, ActionHandlerTargets.Build, null, () => { });
                    }
                }
            }

            return context;
        }
        #endregion

        #region OverrideLogger()
        /// <summary>
        /// Overrides the default logger.
        /// </summary>
        /// <param name="newLogger">The new logger.</param>
        public static void OverrideLogger(Logger newLogger)
        {
            Logger = newLogger;
        }
        #endregion

        #region ResetLogger()
        /// <summary>
        /// Resets the logger to the default.
        /// </summary>
        public static void ResetLogger()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }
        #endregion
        #endregion

        #region Private methods
        #region RetrieveDefaultAction()
        /// <summary>
        /// Retrieves the default action for a level.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="level">The level.</param>
        private static void RetrieveDefaultAction(ActionRequestContext context, ReportLevel level)
        {
            var defaultAction = level.Reports.FirstOrDefault(rep => rep.IsDefault);
            if (defaultAction != null)
            {
                Logger.Debug("Action is default " + level.Target + " level report");
                context.Report = defaultAction.ActionName;
            }
            else
            {
                Logger.Warn("Unable to find default action for " + level.Target + " level!");
                context.Report = "!!unknownAction!!";
            }
        }
        #endregion

        #region CheckLevelForAction()
        /// <summary>
        /// Checks a level for the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="levelToCheck">The level to check.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="clearProperty">The clear property.</param>
        /// <returns>
        /// <c>true</c> if the level was matched; <c>false</c> otherwise.
        /// </returns>
        private static bool CheckLevelForAction(ActionRequestContext context, 
            ActionHandlerTargets levelToCheck,
            string actionName,
            Action clearProperty)
        {
            context.Level = levelToCheck;
            var reportLevel = Manager.Current.ReportLevels.FirstOrDefault(level => level.Target == levelToCheck) ??
                new ReportLevel();
            if (actionName == null)
            {
                RetrieveDefaultAction(context, reportLevel);
                return true;
            }

            var action = reportLevel
                .Reports
                .FirstOrDefault(
                    rep =>
                    string.Equals(rep.ActionName, actionName, StringComparison.InvariantCultureIgnoreCase));
            if (action != null)
            {
                context.Report = actionName;
                clearProperty();
                return true;
            }

            return false;
        }
        #endregion
        #endregion
    }
}
