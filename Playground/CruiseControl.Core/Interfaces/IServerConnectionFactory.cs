namespace CruiseControl.Core.Interfaces
{
    using Common;

    /// <summary>
    /// Factory methods for connecting to remote servers.
    /// </summary>
    public interface IServerConnectionFactory
    {
        #region Public methods
        #region GenerateConnection()
        /// <summary>
        /// Generates a connection to the specified address.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <returns>
        /// The <see cref="ServerConnection"/> for connecting to the server.
        /// </returns>
        ServerConnection GenerateConnection(string address);
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
        string GenerateUrn(string address, string relativeUrn);
        #endregion
        #endregion
    }
}
