namespace CruiseControl.Core.Utilities
{
    using System.Collections.Generic;
    using Common;
    using Interfaces;

    /// <summary>
    /// Factory methods for connecting to remote servers.
    /// </summary>
    public class ServerConnectionFactory
        : IServerConnectionFactory
    {
        #region Private fields
        private readonly IDictionary<string, string> serverNames = new Dictionary<string, string>();
        private readonly object lockObject = new object();
        #endregion

        #region Public methods
        #region GenerateConnection()
        /// <summary>
        /// Generates a connection to the specified address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>
        /// The <see cref="ServerConnection"/> for connecting to the server.
        /// </returns>
        public virtual ServerConnection GenerateConnection(string address)
        {
            var connection = new ServerConnection(address);
            return connection;
        }
        #endregion

        #region GenerateUrn()
        /// <summary>
        /// Generates an absolute URN pointing to an item on the remote server.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="relativeUrn">The relative URN.</param>
        /// <returns>
        /// The absolute URN on the remote server.
        /// </returns>
        public string GenerateUrn(string address, string relativeUrn)
        {
            if (!this.serverNames.ContainsKey(address))
            {
                lock (this.lockObject)
                {
                    if (!this.serverNames.ContainsKey(address))
                    {
                        var serverName = this.GenerateConnection(address).RetrieveServerName();
                        this.serverNames.Add(address, serverName);
                    }
                }
            }

            var baseName = this.serverNames[address];
            if (relativeUrn.StartsWith(":"))
            {
                return baseName + relativeUrn;
            }

            return baseName + ":" + relativeUrn;
        }
        #endregion
        #endregion
    }
}
