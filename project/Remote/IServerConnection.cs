using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A channel for communicating to a remote server.
    /// </summary>
    public interface IServerConnection
    {
        #region Properties
        #region Type
        /// <summary>
        /// The type of connection.
        /// </summary>
        string Type { get; }
        #endregion

        #region ServerName
        /// <summary>
        /// The name of the server that this connection is for.
        /// </summary>
        string ServerName { get; }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this connection busy performing an operation.
        /// </summary>
        bool IsBusy { get; }
        #endregion

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        string Address { get; }
        #endregion
        #endregion

        #region Methods
        #region SendMessage()
        /// <summary>
        /// Sends a message to a remote server.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        /// <returns>The response from the server.</returns>
        Response SendMessage(string action, ServerRequest request);
        #endregion

        #region SendMessageAsync()
        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        void SendMessageAsync(string action, ServerRequest request);

        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        /// <param name="userState">Any user state data.</param>
        void SendMessageAsync(string action, ServerRequest request, object userState);
        #endregion

        #region CancelAsync()
        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        void CancelAsync();

        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        /// <param name="userState"></param>
        void CancelAsync(object userState);
        #endregion
        #endregion

        #region Public events
        #region SendMessageCompleted
        /// <summary>
        /// A SendMessageAsync has completed.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> SendMessageCompleted;
        #endregion

        #region RequestSending
        /// <summary>
        /// A request message is being sent.
        /// </summary>
        event EventHandler<CommunicationsEventArgs> RequestSending;
        #endregion

        #region ResponseReceived
        /// <summary>
        /// A response message has been received.
        /// </summary>
        event EventHandler<CommunicationsEventArgs> ResponseReceived;
        #endregion
        #endregion
    }
}
