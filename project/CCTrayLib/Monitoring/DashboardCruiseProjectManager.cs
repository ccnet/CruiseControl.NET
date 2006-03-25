using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class DashboardCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
		private readonly Uri serverUri;
		private readonly IWebRetriever webRetriver;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public DashboardCruiseProjectManager(IWebRetriever webRetriver, IDashboardXmlParser dashboardXmlParser, Uri serverUri, string projectName)
		{
			this.projectName = projectName;
			this.webRetriver = webRetriver;
			this.dashboardXmlParser = dashboardXmlParser;
			this.serverUri = serverUri;
		}

		public void ForceBuild()
		{
			throw new NotImplementedException("Force build not currently supported on dashboard projects");
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
				string content = webRetriver.Get(serverUri);
				return dashboardXmlParser.ExtractAsProjectStatus(content, projectName);
			}
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}