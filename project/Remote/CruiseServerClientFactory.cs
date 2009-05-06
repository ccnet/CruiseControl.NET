using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Factory class for building <see cref="CruiseServerClient"/> instances.
    /// </summary>
    public static class CruiseServerClientFactory
    {
        #region Public methods
        #region GenerateClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateClient(string address)
        {
            Uri serverUri = new Uri(address);
            IServerConnection connection = null;
            switch (serverUri.Scheme.ToLower())
            {
                case "http":
                    connection = new HttpConnection(address);
                    break;
                case "tcp":
                    connection = new RemotingConnection(address);
                    break;
                default:
                    throw new ApplicationException("Unknown transport protocol");
            }
            CruiseServerClient client = new CruiseServerClient(connection);
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateClient(string address, string targetServer)
        {
            CruiseServerClient client = GenerateClient(address);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion

        #region GenerateHttpClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/> that connects via
        /// HTTP.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateHttpClient(string address)
        {
            IServerConnection connection = new HttpConnection(address);
            CruiseServerClient client = new CruiseServerClient(connection);
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/> that connects via
        /// HTTP to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateHttpClient(string address, string targetServer)
        {
            CruiseServerClient client = GenerateHttpClient(address);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion

        #region GenerateRemotingClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/> that connects via
        /// .NET Remoting.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateRemotingClient(string address)
        {
            IServerConnection connection = new RemotingConnection(address);
            CruiseServerClient client = new CruiseServerClient(connection);
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClient"/> that connects via
        /// .NET Remoting to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClient"/> instance.</returns>
        public static CruiseServerClient GenerateRemotingClient(string address, string targetServer)
        {
            CruiseServerClient client = GenerateRemotingClient(address);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion
        #endregion
    }
}
