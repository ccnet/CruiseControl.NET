using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
	{
		private ICruiseManagerFactory cruiseManagerFactory;
		private IWebRetriever webRetriever;

		public CruiseProjectManagerFactory(ICruiseManagerFactory cruiseManagerFactory, IWebRetriever webRetriever)
		{
			this.cruiseManagerFactory = cruiseManagerFactory;
			this.webRetriever = webRetriever;
		}

		public ICruiseProjectManager Create(CCTrayProject project)
		{
			BuildServer server = project.BuildServer;
			if (server.Transport == BuildServerTransport.Remoting)
			{
				return new RemotingCruiseProjectManager(cruiseManagerFactory.GetCruiseManager(server.Url), project.ProjectName);
			}
			else
			{
				return new HttpCruiseProjectManager(webRetriever, new DashboardXmlParser(), server.Uri, project.ProjectName);
			}
		}

		public CCTrayProject[] GetProjectList(BuildServer server)
		{
			if (server.Transport == BuildServerTransport.Remoting)
			{
				return new RemotingProjectListRetriever(cruiseManagerFactory).GetProjectList(server);
			}
			else
			{
				return new HttpProjectListRetriever(webRetriever, new DashboardXmlParser()).GetProjectList(server);
			}
		}
	}
}