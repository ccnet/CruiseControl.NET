using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin
{
	public class ServerLogViewer : IPlugin
	{
		public string Description
		{
			get { return "View Server Log"; }
		}
		public string Url
		{
			get { return "ViewServerLog.aspx"; }
		}
		public PluginBehavior Behavior
		{
			get { return PluginBehavior.Server;}
		}
	}
}
