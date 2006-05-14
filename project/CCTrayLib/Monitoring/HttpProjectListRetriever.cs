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

		public Project[] GetProjectList(BuildServer server)
		{
			string xml = webRetriver.Get(server.Uri);
			string[] projectNames = dashboardXmlParser.ExtractProjectNames(xml);

			Project[] projects = new Project[projectNames.Length];
			for (int i = 0; i < projectNames.Length; i++)
			{
				projects[i] = new Project(server, projectNames[i]);
			}
			
			return projects;
		}
	}
}