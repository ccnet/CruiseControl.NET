using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpProjectListRetriever
	{
		private readonly IWebRetriever webRetriver;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpProjectListRetriever(IWebRetriever webRetriver, IDashboardXmlParser dashboardXmlParser)
		{
			this.webRetriver = webRetriver;
			this.dashboardXmlParser = dashboardXmlParser;
		}

		public CCTrayProject[] GetProjectList(BuildServer server)
		{
			string xml = webRetriver.Get(server.Uri);
			string[] projectNames = dashboardXmlParser.ExtractProjectNames(xml);

			CCTrayProject[] projects = new CCTrayProject[projectNames.Length];
			for (int i = 0; i < projectNames.Length; i++)
			{
				projects[i] = new CCTrayProject(server, projectNames[i]);
			}
			
			return projects;
		}
	}
}