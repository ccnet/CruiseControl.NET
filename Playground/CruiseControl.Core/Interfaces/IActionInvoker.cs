namespace CruiseControl.Core.Interfaces
{
    using CruiseControl.Common.Messages;

    public interface IActionInvoker
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

        #region Public methods
        #region Invoke()
        /// <summary>
        /// Invokes an action on an item.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <param name="action">The action name.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The return message from the action.
        /// </returns>
        BaseMessage Invoke(string name, string action, BaseMessage message);
        #endregion

        #region List()
        /// <summary>
        /// Lists the available actions on an item.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        RemoteActionDefinition[] List(string name);
        #endregion

        #region Query()
        /// <summary>
        /// Queries the details on an item action.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <param name="action">The action name.</param>
        RemoteActionDefinition Query(string name, string action);
        #endregion
        #endregion
    }
}