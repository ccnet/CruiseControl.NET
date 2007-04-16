using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
		private readonly Uri serverUri;
		private readonly IWebRetriever webRetriever;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpCruiseProjectManager(IWebRetriever webRetriever, IDashboardXmlParser dashboardXmlParser, 
            Uri serverUri, string projectName)
		{
			this.projectName = projectName;
			this.webRetriever = webRetriever;
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

		public void CancelPendingRequest()
		{
			throw new NotImplementedException("Cancel pending not currently supported on projects monitored via HTTP");
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}
