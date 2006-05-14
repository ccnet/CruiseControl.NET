using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
		private readonly Uri serverUri;
		private readonly IWebRetriever webRetriver;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpCruiseProjectManager(IWebRetriever webRetriver, IDashboardXmlParser dashboardXmlParser, Uri serverUri, string projectName)
		{
			this.projectName = projectName;
			this.webRetriver = webRetriver;
			this.dashboardXmlParser = dashboardXmlParser;
			this.serverUri = serverUri;
		}

		public void ForceBuild()
		{
			throw new NotImplementedException("Force build not currently supported on projects monitored via HTTP");
		}

		public void FixBuild()
		{
			throw new NotImplementedException("Fix build not currently supported on projects monitored via HTTP");
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