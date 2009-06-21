using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Factory class for building <see cref="CruiseServerClientBase"/> instances.
    /// </summary>
    public class CruiseServerClientFactory 
        : ICruiseServerClientFactory
    {
        #region Private fields
        private Dictionary<string, ClientInitialiser> initialisers = new Dictionary<string, ClientInitialiser>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="CruiseServerClientFactory"/>.
        /// </summary>
        public CruiseServerClientFactory()
        {
            // Add the initial initialisers
            InitialiseDefaultTcpClient();
            InitialiseDefaultHttpClient();
        }
        #endregion

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
            var serverUri = new Uri(address);
            var transport = serverUri.Scheme.ToLower();
            if (initialisers.ContainsKey(transport))
            {
                var client = initialisers[transport](address, settings);
                return client;
            }
            else
            {
                throw new ApplicationException("Unknown transport protocol");
            }
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

        #region InitialiseDefaultHttpClient()
        /// <summary>
        /// Initialise the default HTTP client (via Web Dashboard).
        /// </summary>
        public void InitialiseDefaultHttpClient()
        {
            initialisers["http"] = (address, settings) =>
            {
                if (settings.BackwardsCompatable)
                {
                    return new CruiseServerHttpClient(address);
                }
                else
                {
                    return new CruiseServerClient(new HttpConnection(address));
                }
            };
        }
        #endregion

        #region InitialiseDefaultTcpClient()
        /// <summary>
        /// Initialise the default TCP client (via .NET Remoting).
        /// </summary>
        public void InitialiseDefaultTcpClient()
        {
            initialisers["tcp"] = (address, settings) =>
            {
                if (settings.BackwardsCompatable)
                {
                    return new CruiseServerRemotingClient(address);
                }
                else
                {
                    return new CruiseServerClient(new RemotingConnection(address));
                }
            };
        }
        #endregion

        #region AddInitialiser()
        /// <summary>
        /// Adds a transport initialiser.
        /// </summary>
        /// <param name="transport">The transport to initialise.</param>
        /// <param name="initialiser">The new initialiser.</param>
        public void AddInitialiser(string transport, ClientInitialiser initialiser)
        {
            initialisers[transport.ToLower()] = initialiser;
        }
        #endregion
        #endregion

        #region Public delegates
        #region ClientInitialiser
        /// <summary>
        /// Initialises a client.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="settings"></param>
        public delegate CruiseServerClientBase ClientInitialiser(string address, ClientStartUpSettings settings);
        #endregion
        #endregion
    }
}
