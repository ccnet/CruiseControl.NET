using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    [ReflectorType("serverAuditHistoryServerPlugin")]
    public class ServerAuditHistoryServerPlugin : ICruiseAction, IPlugin
    {
        private const string ActionName = "ViewServerAuditHistory";
        private const string DiagnosticsActionName = "ViewAuditRecord";
        private readonly IFarmService farmService;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ISessionRetriever sessionRetriever;
        private readonly IUrlBuilder urlBuilder;

        public ServerAuditHistoryServerPlugin(IFarmService farmService, 
            IVelocityViewGenerator viewGenerator, 
            ISessionRetriever sessionRetriever,
            IUrlBuilder urlBuilder)
        {
            this.farmService = farmService;
            this.viewGenerator = viewGenerator;
            this.sessionRetriever = sessionRetriever;
            this.urlBuilder = urlBuilder;
        }

        public IResponse Execute(ICruiseRequest request)
        {
            return GenerateAuditHistory(request);
        }

        private IResponse GenerateAuditHistory(ICruiseRequest request)
        {
            Hashtable velocityContext = new Hashtable();

            ArrayList links = new ArrayList();
            links.Add(new ServerLink(request.UrlBuilder, request.ServerSpecifier, "Server", ActionName));

            ProjectStatusListAndExceptions projects = farmService.GetProjectStatusListAndCaptureExceptions(request.ServerSpecifier, request.RetrieveSessionToken());
            foreach (ProjectStatusOnServer projectStatusOnServer in projects.StatusAndServerList)
            {
                DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(projectStatusOnServer.ServerSpecifier, projectStatusOnServer.ProjectStatus.Name);
                links.Add(new ProjectLink(request.UrlBuilder, projectSpecifier, projectSpecifier.ProjectName, ServerAuditHistoryServerPlugin.ActionName));
            }
            velocityContext["projectLinks"] = links;
            string sessionToken = request.RetrieveSessionToken(sessionRetriever);
            if (!string.IsNullOrEmpty(request.ProjectName))
            {
                velocityContext["currentProject"] = request.ProjectName;
                AuditFilterBase filter = AuditFilters.ByProject(request.ProjectName);
                velocityContext["auditHistory"] = farmService.ReadAuditRecords(request.ServerSpecifier, sessionToken, 0, 100, filter);
            }
            else
            {
                velocityContext["auditHistory"] = new ServerLink(request.UrlBuilder, request.ServerSpecifier, string.Empty, DiagnosticsActionName);
                velocityContext["auditHistory"] = farmService.ReadAuditRecords(request.ServerSpecifier, sessionToken, 0, 100);
            }

            return viewGenerator.GenerateView(@"AuditHistory.vm", velocityContext);
        }

        public string LinkDescription
        {
            get { return "View Audit History"; }
        }

        public INamedAction[] NamedActions
        {
            get
            {
                return new INamedAction[] { new ImmutableNamedAction(ActionName, this) };
            }
        }
    }
}
