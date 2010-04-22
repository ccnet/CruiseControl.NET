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

    /// <title>Project Status Plugin</title>
    /// <version>1.0</version>
    /// <summary>
    /// Displays the status of a project.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;viewProjectStatusPlugin /&gt;
    /// </code>
    /// </example>
    [ReflectorType("viewProjectStatusPlugin")]
    public class ProjectStatusPlugin : ICruiseAction, IPlugin
    {
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly IFarmService farmServer;
        private readonly ICruiseUrlBuilder urlBuilder;

        public ProjectStatusPlugin(IFarmService farmServer, IVelocityViewGenerator viewGenerator, ICruiseUrlBuilder urlBuilder)
        {
            this.farmServer = farmServer;
            this.viewGenerator = viewGenerator;
            this.urlBuilder = urlBuilder;
        }

        /// <summary>
        /// Executes the specified cruise request.
        /// </summary>
        /// <param name="cruiseRequest">The cruise request.</param>
        /// <returns></returns>
        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            var projectSpecifier = cruiseRequest.ProjectSpecifier;
            var velocityContext = new Hashtable();
            velocityContext["dataUrl"] = urlBuilder.BuildProjectUrl(ProjectStatusAction.ActionName, projectSpecifier) + "?view=json";
            velocityContext["projectName"] = projectSpecifier.ProjectName;
            if (cruiseRequest.Request.ApplicationPath == "/")
            {
                velocityContext["applicationPath"] = string.Empty;
            }
            else
            {
                velocityContext["applicationPath"] = cruiseRequest.Request.ApplicationPath;
            }
            return viewGenerator.GenerateView(@"ProjectStatusReport.vm", velocityContext);
        }

        public string LinkDescription
        {
            get { return "Project Status"; }
        }

        public INamedAction[] NamedActions
        {
            get { return new INamedAction[] { new ImmutableNamedAction("ViewProjectStatus", this) }; }
        }
    }
}
