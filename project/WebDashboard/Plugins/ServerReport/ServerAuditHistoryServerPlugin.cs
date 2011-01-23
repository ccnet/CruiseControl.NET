using System.Collections;
using System.Collections.Generic;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
    /// <title>Server Audit History Server Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// The Server Audit History Server Plugin displays the audit log from the server.
    /// server is running.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;serverAuditHistoryServerPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// This requires that the currently logged in user has the required permissions on the server.
    /// </remarks>
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
            var velocityContext = new Hashtable();
            var links = new List<IAbsoluteLink>();
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
