using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseServerManagerFactory : ICruiseServerManagerFactory
	{
		private readonly ICruiseManagerFactory cruiseManagerFactory;

		public CruiseServerManagerFactory(ICruiseManagerFactory cruiseManagerFactory)
		{
			this.cruiseManagerFactory = cruiseManagerFactory;
		}

		public ICruiseServerManager Create(BuildServer buildServer)
		{		
			if (buildServer.Transport == BuildServerTransport.Remoting)
			{
                return new CachingCruiseServerManager(
                    new RemotingCruiseServerManager(cruiseManagerFactory.GetCruiseManager(buildServer.Url), buildServer));
			}
			else
			{

				return new CachingCruiseServerManager(
					new HttpCruiseServerManager(new WebRetriever(), new DashboardXmlParser(), buildServer));
			}
		}
	}
}
