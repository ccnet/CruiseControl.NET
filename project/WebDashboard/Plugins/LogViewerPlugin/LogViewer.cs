using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewer : IPlugin
	{
		public string Description
		{
			get { return "View Log"; }
		}
		public string Url
		{
			get { return "ViewLog.aspx"; }
		}
		public PluginBehavior Behavior
		{
			get { return PluginBehavior.Build; }
		}
	}
}
