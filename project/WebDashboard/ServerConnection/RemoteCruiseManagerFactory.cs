using System;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class RemoteCruiseManagerFactory : ICruiseManagerFactory
	{
		// This is not unit tested right now
		public ICruiseManager GetCruiseManager(string url)
		{
			return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager),url);
		}
	}
}
