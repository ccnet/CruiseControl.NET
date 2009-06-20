namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Extension methods to <see cref="CruiseServerClientFactory"/> to allow generating additional
    /// client connection types.
    /// </summary>
    public static class CruiseServerClientFactoryExtensions
    {
        #region Public methods
        #region GenerateWcfClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// Windows Communications Foundation.
        /// </summary>
        /// <param name="factory">The <see cref="CruiseServerClientFactory"/> that is being extended.</param>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public static CruiseServerClientBase GenerateWcfClient(this CruiseServerClientFactory factory, string address)
        {
            var client = GenerateWcfClient(factory, address, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// Windows Communications Foundation to another server.
        /// </summary>
        /// <param name="factory">The <see cref="CruiseServerClientFactory"/> that is being extended.</param>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public static CruiseServerClientBase GenerateWcfClient(this CruiseServerClientFactory factory, string address, string targetServer)
        {
            var client = GenerateWcfClient(factory, address, targetServer, new ClientStartUpSettings());
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// Windows Communications Foundation.
        /// </summary>
        /// <param name="factory">The <see cref="CruiseServerClientFactory"/> that is being extended.</param>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public static CruiseServerClientBase GenerateWcfClient(this CruiseServerClientFactory factory, string address, ClientStartUpSettings settings)
        {
            CruiseServerClientBase client;
            var connection = new WcfConnection(address);
            client = new CruiseServerClient(connection);
            return client;
        }

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// Windows Communications Foundation to another server.
        /// </summary>
        /// <param name="factory">The <see cref="CruiseServerClientFactory"/> that is being extended.</param>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        public static CruiseServerClientBase GenerateWcfClient(this CruiseServerClientFactory factory, string address, string targetServer, ClientStartUpSettings settings)
        {
            var client = GenerateWcfClient(factory, address, settings);
            client.TargetServer = targetServer;
            return client;
        }
        #endregion
        #endregion
    }
}
