using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IRemotingService
	{
        /// <summary>
        /// Connects the specified proxy type.	
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		object Connect(Type proxyType, string uri);
	}
}
