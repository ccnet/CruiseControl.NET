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
    /// <title>Server Log Project Plugin</title>
    /// <version>1.0</version>
    /// <summary>
    /// The Server Log Project Plugin shows you recent activity that has been output to the server log for a specific project. 
    /// <para>
    /// LinkDescription : View Server Log.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;serverLogProjectPlugin /&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// Read the <link>Server Application Config File</link> page for more help on build server logging.
    /// </remarks>
    [ReflectorType("serverLogProjectPlugin")]
	public class ServerLogProjectPlugin : ICruiseAction, IPlugin
	{
		public const string ActionName = "ViewServerProjectLog";
		private readonly ServerLogServerPlugin plugin;

        public ServerLogProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator, ICruiseUrlBuilder urlBuilder)
		{
			plugin = new ServerLogServerPlugin(farmService, viewGenerator, urlBuilder);
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