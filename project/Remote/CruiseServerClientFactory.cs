using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Factory class for building <see cref="CruiseServerClientBase"/> instances.
    /// </summary>
    public class CruiseServerClientFactory 
        : ICruiseServerClientFactory
    {
        #region Public methods
        #region GenerateClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateClient(string address)
        {
            var client = GenerateClient(address, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateClient(string address, string targetServer)
        {
            var client = GenerateClient(address, targetServer, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateClient(string address, ClientStartUpSettings settings)
        {
            CruiseServerClientBase client; 
            var serverUri = new Uri(address);
            if (settings.BackwardsCompatable)
            {
                switch (serverUri.Scheme.ToLower())
                {
                    case "http":
                        client = new CruiseServerHttpClient(address);
                        break;
                    case "tcp":
                        client = new CruiseServerRemotingClient(address);
                        break;
                    default:
                        throw new ApplicationException("Unknown transport protocol");
                }
            }
            else
            {
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
                client = new CruiseServerClient(connection);
            }
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateClient(string address, string targetServer, ClientStartUpSettings settings)
        {
            var client = GenerateClient(address, settings);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion

        #region GenerateHttpClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateHttpClient(string address)
        {
            var client = GenerateHttpClient(address, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateHttpClient(string address, string targetServer)
        {
            var client = GenerateHttpClient(address, targetServer, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateHttpClient(string address, ClientStartUpSettings settings)
        {
            CruiseServerClientBase client;
            if (settings.BackwardsCompatable)
            {
                client = new CruiseServerHttpClient(address);
            }
            else
            {
                var connection = new HttpConnection(address);
                client = new CruiseServerClient(connection);
            }
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateHttpClient(string address, string targetServer, ClientStartUpSettings settings)
        {
            var client = GenerateHttpClient(address, settings);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion

        #region GenerateRemotingClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateRemotingClient(string address)
        {
            var client = GenerateRemotingClient(address, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateRemotingClient(string address, string targetServer)
        {
            var client = GenerateRemotingClient(address, targetServer, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateRemotingClient(string address, ClientStartUpSettings settings)
        {
            CruiseServerClientBase client;
            if (settings.BackwardsCompatable)
            {
                client = new CruiseServerRemotingClient(address);
            }
            else
            {
                var connection = new RemotingConnection(address);
                client = new CruiseServerClient(connection);
            }
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public CruiseServerClientBase GenerateRemotingClient(string address, string targetServer, ClientStartUpSettings settings)
        {
            var client = GenerateRemotingClient(address, settings);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion
        #endregion
    }
}
