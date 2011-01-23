using System;
using System.Runtime.Remoting;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// 	
    /// </summary>
	public class RemotingServiceAdapter : IRemotingService
	{
        /// <summary>
        /// Connects the specified proxy type.	
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public object Connect(Type proxyType, string uri)
		{
			return RemotingServices.Connect(proxyType, uri);
		}
	}
}