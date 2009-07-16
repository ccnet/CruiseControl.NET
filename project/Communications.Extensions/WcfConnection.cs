using System;
using System.ServiceModel;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A server connection using Windows Communications Foundation.
    /// </summary>
    public class WcfConnection
        : IServerConnection, IDisposable
    {
        #region Private fields
        private Uri serverAddress;
        private CruiseControlContractClient client;
        private bool isBusy;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new <see cref="WcfConnection"/> to a remote server.
        /// </summary>
        /// <param name="serverAddress">The address of the remote server.</param>
        public WcfConnection(string serverAddress)
            : this(new Uri(serverAddress))
        {
        }

        /// <summary>
        /// Initialises a new <see cref="WcfConnection"/> to a remote server.
        /// </summary>
        /// <param name="serverAddress">The address of the remote server.</param>
        public WcfConnection(Uri serverAddress)
        {
            this.serverAddress = serverAddress;
        }
        #endregion

        #region Public properties
        #region Type
        /// <summary>
        /// The type of connection.
        /// </summary>
        public string Type
        {
            get { return "Windows Communication Foundation"; }
        }
        #endregion

        #region ServerName
        /// <summary>
        /// The name of the server that this connection is for.
        /// </summary>
        public string ServerName
        {
            get { return client.Endpoint.ListenUri.Host; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this connection busy performing an operation.
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
        }
        #endregion

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        public virtual string Address
        {
            get { return serverAddress.AbsoluteUri; }
        }
        #endregion
        #endregion

        #region Public methods
        #region SendMessage()
        /// <summary>
        /// Sends a message via HTTP.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Response SendMessage(string action, ServerRequest request)
        {
            // Initialise the connection and send the message
            InitialiseClient();
            var result = client.ProcessMessage(action, request);
            return result;
        }
        #endregion

        #region SendMessageAsync()
        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        public virtual void SendMessageAsync(string action, ServerRequest request)
        {
            SendMessageAsync(action, request, null);
        }

        /// <summary>
        /// Sends a message to a remote server asychronously.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="request">The request to send to the server.</param>
        /// <param name="userState">Any user state data.</param>
        /// <remarks>
        /// This operation will still be done in a synchronous mode.
        /// </remarks>
        public virtual void SendMessageAsync(string action, ServerRequest request, object userState)
        {
            if (isBusy) throw new InvalidOperationException();

            try
            {
                isBusy = true;
                InitialiseClient();
                IAsyncResult async = null;
                async = client.BeginProcessMessage(action, request, (result) =>
                {
                    if (SendMessageCompleted != null)
                    {
                        var response = client.EndProcessMessage(async);
                        var args = new MessageReceivedEventArgs(response, null, false, userState);
                        SendMessageCompleted(this, args);
                    }
                    isBusy = false;
                }, userState);

            }
            catch (Exception error)
            {
                if (SendMessageCompleted != null)
                {
                    var args = new MessageReceivedEventArgs(null, error, false, userState);
                    SendMessageCompleted(this, args);
                }
                isBusy = false;
            }
        }
        #endregion

        #region CancelAsync()
        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        public virtual void CancelAsync()
        {
            CancelAsync(null);
        }

        /// <summary>
        /// Cancels an asynchronous operation.
        /// </summary>
        /// <param name="userState"></param>
        public void CancelAsync(object userState)
        {
            if (isBusy) client.Abort();
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Disposes the .NET remoting client.
        /// </summary>
        public virtual void Dispose()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
        #endregion
        #endregion

        #region Public events
        #region SendMessageCompleted
        /// <summary>
        /// A SendMessageAsync has completed.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> SendMessageCompleted;
        #endregion
        #endregion

        #region Private methods
        #region InitialiseRemoting()
        /// <summary>
        /// Initialises the client connection.
        /// </summary>
        private void InitialiseClient()
        {
            if ((client != null) && (client.State != CommunicationState.Opened))
            {
                Dispose();
            }

            if (client == null)
            {
                client = new CruiseControlContractClient(new BasicHttpBinding(),
                    new EndpointAddress(serverAddress));
                client.Open();
            }
        }
        #endregion
        #endregion
    }
}
