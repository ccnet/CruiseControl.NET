using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
	{
		private ICruiseManagerFactory cruiseManagerFactory;

		public CruiseProjectManagerFactory(ICruiseManagerFactory cruiseManagerFactory)
		{
			this.cruiseManagerFactory = cruiseManagerFactory;
		}

		public ICruiseProjectManager Create(Project project)
		{		
			BuildServer server = project.BuildServer;
			if (server.Transport == BuildServerTransport.Remoting)
			{
				return new RemotingCruiseProjectManager(cruiseManagerFactory.GetCruiseManager(server.Url), project.ProjectName);
			}
			else
			{
				return new HttpCruiseProjectManager(new WebRetriever(), new DashboardXmlParser(), server.Uri, project.ProjectName);
			}
		}

		public Project[] GetProjectList(BuildServer server)
		{
			if (server.Transport == BuildServerTransport.Remoting)
			{
				return new RemotingProjectListRetriever(cruiseManagerFactory).GetProjectList(server);
			}
			else
			{
				return new HttpProjectListRetriever(new WebRetriever(), new DashboardXmlParser()).GetProjectList(server);
			}
		}
	}
}