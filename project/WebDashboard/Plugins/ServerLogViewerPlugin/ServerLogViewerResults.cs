using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin
{
	public class ServerLogViewerResults
	{
		private readonly string log;

		public ServerLogViewerResults(string log)
		{
			this.log = log;
		}

		public string LogHtml
		{
			get { return log; }
		}
	}
}
