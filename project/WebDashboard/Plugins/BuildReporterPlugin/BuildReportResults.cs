using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReporterPlugin
{
	public class BuildReportResults
	{
		private readonly string html;
		
		public string Html
		{
			get { return html; }
		}

		public BuildReportResults(string html)
		{
			this.html = html;
		}
	}
}
