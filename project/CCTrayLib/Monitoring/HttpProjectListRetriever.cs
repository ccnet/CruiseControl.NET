using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpProjectListRetriever
	{
		private readonly IWebRetriever webRetriever;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpProjectListRetriever(IWebRetriever webRetriever, IDashboardXmlParser dashboardXmlParser)
		{
			this.webRetriever = webRetriever;
			this.dashboardXmlParser = dashboardXmlParser;
		}

		public CCTrayProject[] GetProjectList(BuildServer server)
		{
			string xml = webRetriever.Get(server.Uri);
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