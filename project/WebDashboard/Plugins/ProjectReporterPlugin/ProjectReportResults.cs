using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class ProjectReportResults
	{
		private readonly string html;
		
		public string Html
		{
			get { return html; }
		}

		public ProjectReportResults(string html)
		{
			this.html = html;
		}
	}
}
