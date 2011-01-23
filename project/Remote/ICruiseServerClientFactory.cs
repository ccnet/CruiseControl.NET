using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Factory for building <see cref="CruiseServerClientBase"/> instances.
    /// </summary>
    public interface ICruiseServerClientFactory
    {
        #region Public methods
        #region GenerateClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateClient(string address, ClientStartUpSettings settings);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateClient(string address, string targetServer, ClientStartUpSettings settings);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateClient(string address);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/>. The transport protocol will be
        /// detected from the address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateClient(string address, string targetServer);
        #endregion

        #region GenerateHttpClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateHttpClient(string address, string targetServer);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateHttpClient(string address);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateHttpClient(string address, ClientStartUpSettings settings);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// HTTP to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateHttpClient(string address, string targetServer, ClientStartUpSettings settings);
        #endregion

        #region GenerateRemotingClient()
        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateRemotingClient(string address, ClientStartUpSettings settings);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateRemotingClient(string address, string targetServer, ClientStartUpSettings settings);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateRemotingClient(string address);

        /// <summary>
        /// Generates an instance of <see cref="CruiseServerClientBase"/> that connects via
        /// .NET Remoting to another server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="targetServer">The name of the other server.</param>
        /// <returns>A <see cref="CruiseServerClientBase"/> instance.</returns>
        CruiseServerClientBase GenerateRemotingClient(string address, string targetServer);
        #endregion

        #region ResetCache()
        /// <summary>
        /// Resets the entire client cache.
        /// </summary>
        void ResetCache();

        /// <summary>
        /// Resets the cache for a client address.
        /// </summary>
        /// <param name="address">The address to reset.</param>
        void ResetCache(string address);
        #endregion
        #endregion
    }
}
