using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
    /// <title>Server Audit History Project Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// The Server Audit History Project Plugin displays the audit log from the server.
    /// server is running.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;serverAuditHistoryProjectPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// This requires that the currently logged in user has the required permissions on the server.
    /// </remarks>
    [ReflectorType("serverAuditHistoryProjectPlugin")]
	public class ServerAuditHistoryProjectPlugin : ICruiseAction, IPlugin
	{
        public const string ActionName = "ViewProjectAuditHistory";
        private readonly ServerAuditHistoryServerPlugin plugin;

		public ServerAuditHistoryProjectPlugin(IFarmService farmService, 
            IVelocityViewGenerator viewGenerator, 
            ISessionRetriever sessionRetriever,
            IUrlBuilder urlBuilder)
		{
            plugin = new ServerAuditHistoryServerPlugin(farmService, viewGenerator, sessionRetriever, urlBuilder);
		}

		public IResponse Execute(ICruiseRequest request)
		{
			return plugin.Execute(request);
		}

		public string LinkDescription
		{
			get { return plugin.LinkDescription; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ActionName, this) }; }
		}	
	}
}
