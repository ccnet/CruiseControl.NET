using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
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

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;
            Hashtable velocityContext = new Hashtable();
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
