using System;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
	{
		ICruiseManagerFactory cruiseManagerFactory;

		public CruiseProjectManagerFactory( ICruiseManagerFactory cruiseManagerFactory )
		{
			this.cruiseManagerFactory = cruiseManagerFactory;
		}

		public ICruiseProjectManager Create( string serverUrl, string projectName )
		{
			return new CruiseProjectManager(cruiseManagerFactory.GetCruiseManager(serverUrl), projectName);
		}
	}
}
