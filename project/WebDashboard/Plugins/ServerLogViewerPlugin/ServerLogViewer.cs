using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin
{
	public class ServerLogViewer : IServerPlugin
	{
		public string Description
		{
			get { return "View Server Log"; }
		}

		public string CreateURL (string serverName, IServerUrlGenerator urlGenerator)
		{
			return urlGenerator.GenerateUrl("ViewServerLog.aspx", serverName);
		}
	}
}
