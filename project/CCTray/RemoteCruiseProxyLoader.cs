using System;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	public class RemoteCruiseProxyLoader : IRemoteCruiseProxyLoader
	{
		public RemoteCruiseProxyLoader()
		{
		}

		public ICruiseManager LoadProxy(Settings settings)
		{
			if (settings.ConnectionMethod == ConnectionMethod.WebService)
			{
				return new ThoughtWorks.CruiseControl.WebServiceProxy.CCNetManagementProxy(settings.RemoteServerUrl);
			}
			if (settings.ConnectionMethod == ConnectionMethod.Remoting)
			{
				GetRemoteObject(settings);
				return (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), settings.RemoteServerUrl);
			}
			throw new NotImplementedException("Connection method " + settings.ConnectionMethod + " is not implemented.");
		}

	    protected virtual ICruiseManager GetRemoteObject(Settings settings)
	    {
	        return (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), settings.RemoteServerUrl);
	    }
	}
}
