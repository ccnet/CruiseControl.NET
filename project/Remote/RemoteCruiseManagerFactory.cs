
using System.Runtime.Remoting;
using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// 	
    /// </summary>
	public class RemoteCruiseManagerFactory : ICruiseManagerFactory
	{
        private const string managerUri = "CruiseManager.rem";
        private const string serverClientUri = "CruiseServerClient.rem";

        /// <summary>
        /// Gets the cruise manager.	
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [Obsolete("Use GetCruiseServerClient() instead")]
		public ICruiseManager GetCruiseManager(string url)
		{
            // Convert a ServerClient URI to a Manager URI.
            var actualUri = url;
            if (url.EndsWith(serverClientUri, StringComparison.OrdinalIgnoreCase))
            {
                actualUri = url.Substring(0, url.Length - serverClientUri.Length) + managerUri;
            }
            return (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), actualUri);
        }

        /// <summary>
        /// Gets the cruise server client.	
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ICruiseServerClient GetCruiseServerClient(string url)
		{
            // Convert a ServerClient URI to a Manager URI.
            var actualUri = url;
            if (url.EndsWith(managerUri, StringComparison.OrdinalIgnoreCase))
            {
                actualUri = url.Substring(0, url.Length - managerUri.Length) + serverClientUri;
            }
            return (ICruiseServerClient)RemotingServices.Connect(typeof(ICruiseServerClient), actualUri);
		}
	}
}