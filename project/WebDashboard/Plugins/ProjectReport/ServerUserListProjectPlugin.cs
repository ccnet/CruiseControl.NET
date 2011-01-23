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
    /// <title>User List Project Plugin</title>
    /// <version>1.5</version>
    /// <summary>
    /// Displays all the users in the system, plus the security rights they have for the project.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;serverUserListProjectPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para type="tip">
    /// This can be installed using the "User List" package.
    /// </para>
    /// </remarks>
    [ReflectorType("serverUserListProjectPlugin")]
	public class ServerUserListProjectPlugin : ICruiseAction, IPlugin
	{
        public const string ActionName = "ViewProjectUserList";
        private readonly ServerUserListServerPlugin plugin;

		public ServerUserListProjectPlugin(IFarmService farmService, 
            IVelocityViewGenerator viewGenerator, 
            ISessionRetriever sessionRetriever,
            IUrlBuilder urlBuilder)
		{
            plugin = new ServerUserListServerPlugin(farmService, viewGenerator, sessionRetriever, urlBuilder);
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
