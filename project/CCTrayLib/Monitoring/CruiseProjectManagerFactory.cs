using System;
using System.Security.Policy;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;
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

		public ICruiseProjectManager Create(string serverUrl, string projectName)
		{
			Uri serverUri = new Uri(serverUrl);
			if (serverUri.Scheme == "tcp")
			{
				return new CruiseProjectManager(cruiseManagerFactory.GetCruiseManager(serverUrl), projectName);
			}
			
			return new DashboardCruiseProjectManager(new WebRetriever(), new DashboardXmlParser(), serverUri, projectName);
		}
	}

}