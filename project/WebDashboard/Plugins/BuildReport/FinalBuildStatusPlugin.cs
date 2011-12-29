namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    /// <title>Final Build Status Display Plugin</title>
    /// <version>1.6</version>
    /// <summary>
    /// Display the final status of a project after a build.
    /// <para>
    /// LinkDescription : View Final Project Status.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;finalBuildStatusPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This requires that the project on the server has been configured with a data store.
    /// </para>
    /// <para type="tip">
    /// This can be installed using the "FinalBuildStatus" package.
    /// </para>
    /// </remarks>
    [ReflectorType("finalBuildStatusPlugin")]
    public class FinalBuildStatusPlugin
        : ICruiseAction, IBuildPlugin
    {
        #region Private fields
        private const string ActionName = "ViewFinalBuildStatus";
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalBuildStatusPlugin"/> class.
        /// </summary>
        /// <param name="farmService">The farm service.</param>
        /// <param name="viewGenerator">The view generator.</param>
        public FinalBuildStatusPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
        }
        #endregion

        #region Public properties
        #region LinkDescription
        /// <summary>
        /// Gets the link description.
        /// </summary>
        /// <value>The link description.</value>
        public string LinkDescription
        {
            get { return "View Final Project Status"; }
        }
        #endregion

        #region NamedActions
        /// <summary>
        /// Gets the named actions.
        /// </summary>
        /// <value>The named actions.</value>
        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction(ActionName, this) }; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response for the request.</returns>
        public IResponse Execute(ICruiseRequest request)
        {
            var status = farmService.GetFinalBuildStatus(request.BuildSpecifier,
                request.RetrieveSessionToken());
            var velocityContext = new Hashtable();
            velocityContext["status"] = status;
            if (request.Request.ApplicationPath == "/")
            {
                velocityContext["applicationPath"] = string.Empty;
            }
            else
            {
                velocityContext["applicationPath"] = request.Request.ApplicationPath;
            } 
            return viewGenerator.GenerateView(@"FinalBuildStatus.vm", velocityContext);
        }
        #endregion

        #region IsDisplayedForProject()
        /// <summary>
        /// Determines whether this plug-in is displayed for the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// <c>true</c> if this plug-in is displayed for the project; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDisplayedForProject(IProjectSpecifier project)
        {
            return true;
        }
        #endregion
        #endregion
    }
}
