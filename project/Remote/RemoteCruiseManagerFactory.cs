#pragma warning disable 1591
using System.Runtime.Remoting;
using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	public class RemoteCruiseManagerFactory : ICruiseManagerFactory
	{
        private const string managerUri = "CruiseManager.rem";
        private const string serverClientUri = "CruiseServerClient.rem";

        [Obsolete("Use GetCruiseServerClient() instead")]
		public ICruiseManager GetCruiseManager(string url)
		{
            // Convert a ServerClient URI to a Manager URI.
            var actualUri = url;
            if (url.EndsWith(serverClientUri, StringComparison.InvariantCultureIgnoreCase))
            {
                actualUri = url.Substring(0, url.Length - serverClientUri.Length) + managerUri;
            }
            return (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), actualUri);
        }

        public ICruiseServerClient GetCruiseServerClient(string url)
		{
            // Convert a ServerClient URI to a Manager URI.
            var actualUri = url;
            if (url.EndsWith(managerUri, StringComparison.InvariantCultureIgnoreCase))
            {
                actualUri = url.Substring(0, url.Length - managerUri.Length) + serverClientUri;
            }
            return (ICruiseServerClient)RemotingServices.Connect(typeof(ICruiseServerClient), actualUri);
		}
	}
}