using System;
using System.Runtime.Remoting;

namespace ThoughtWorks.CruiseControl.Remote
{
	public class RemotingServiceAdapter : IRemotingService
	{
		public object Connect(Type proxyType, string uri)
		{
			return RemotingServices.Connect(proxyType, uri);
		}

		public bool Disconnect(object remoteObj)
		{
			return RemotingServices.Disconnect((MarshalByRefObject) remoteObj);
		}
	}
}