using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	public interface IRemotingService
	{
		object Connect(Type proxyType, string uri);
		bool Disconnect(object remoteObj);
	}
}
