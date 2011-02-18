namespace CruiseControl.Core.Interfaces
{
    using CruiseControl.Common;

    /// <summary>
    /// Allows the invoking and querying of actions on a server.
    /// </summary>
    public interface IActionInvoker
        : ICommunicationsChannel
    {
        #region Public properties
        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        Server Server { get; set; }
        #endregion
        #endregion
    }
}