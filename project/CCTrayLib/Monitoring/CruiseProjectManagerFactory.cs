using System.Collections.Generic;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
	{
		private readonly ICruiseManagerFactory cruiseManagerFactory;

		public CruiseProjectManagerFactory(ICruiseManagerFactory cruiseManagerFactory)
		{
			this.cruiseManagerFactory = cruiseManagerFactory;
		}

		public ICruiseProjectManager Create(CCTrayProject project, IDictionary<BuildServer, ICruiseServerManager> serverManagers)
		{
			BuildServer server = project.BuildServer;
            switch (server.Transport)
			{
                case BuildServerTransport.Remoting:
				return new RemotingCruiseProjectManager(cruiseManagerFactory.GetCruiseManager(server.Url), project.ProjectName);
                case BuildServerTransport.Extension:
                    ITransportExtension extensionInstance = ExtensionHelpers.RetrieveExtension(server.ExtensionName);
                    extensionInstance.Settings = server.ExtensionSettings;
                    extensionInstance.Configuration = server;
                    return extensionInstance.RetrieveProjectManager(project.ProjectName);
                default:
                return new HttpCruiseProjectManager(new WebRetriever(), project.ProjectName, serverManagers[server]);
			}
		}

		public CCTrayProject[] GetProjectList(BuildServer server)
		{
            switch (server.Transport)
			{
                case BuildServerTransport.Remoting:
				return new RemotingProjectListRetriever(cruiseManagerFactory).GetProjectList(server);
                case BuildServerTransport.Extension:
                    return new ExtensionTransportProjectListRetriever(server.ExtensionName).GetProjectList(server);
                default:
                return new HttpProjectListRetriever(new WebRetriever(), new DashboardXmlParser()).GetProjectList(server);
			}
		}
	}
}
