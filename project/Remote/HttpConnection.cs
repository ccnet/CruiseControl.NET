namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;
    using System.Text;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// A server connection over HTTP.
    /// </summary>
    public class HttpConnection
        : ServerConnectionBase, IServerConnection, IDisposable
    {
        #region Private fields
        private readonly Uri serverAddress;
        private bool isBusy;
        private Dictionary<object, IWebClient> asyncOperations = new Dictionary<object, IWebClient>();
        private object lockObject = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new <see cref="HttpConnection"/> to a remote server.
        /// </summary>
        /// <param name="serverAddress">The address of the remote server.</param>
        public HttpConnection(string serverAddress)
            : this(new Uri(serverAddress))
        {
        }

        /// <summary>
        /// Initialises a new <see cref="HttpConnection"/> to a remote server.
        /// </summary>
        /// <param name="serverAddress">The address of the remote server.</param>
        public HttpConnection(Uri serverAddress)
            : this(serverAddress, new WebClientFactory<DefaultWebClient>())
        {
        }

        /// <summary>
        /// Initialises a new <see cref="HttpConnection"/> to a remote server.
        /// </summary>
        /// <param name="serverAddress">The address of the remote server.</param>
        /// <param name="clientfactory">The clientfactory.</param>
        public HttpConnection(Uri serverAddress, IWebClientFactory clientfactory)
        {
            this.serverAddress = serverAddress;
            this.WebClientfactory = clientfactory;
        }
        #endregion

        #region Public properties
        #region WebClientFactory
        /// <summary>
        /// Gets or sets the web client.
        /// </summary>
        /// <value>The web client.</value>
        public IWebClientFactory WebClientfactory { get; set; }
        #endregion

        #region Type
        /// <summary>
        /// The type of connection.
        /// </summary>
        public string Type
        {
            get { return "HTTP"; }
        }
        #endregion

        #region ServerName
        /// <summary>
        /// The name of the server that this connection is for.
        /// </summary>
        public string ServerName
        {
            get { return serverAddress.Host; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this connection busy performing an operation.
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy || (asyncOperations.Count > 0); }
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
        public Response SendMessage(string action, ServerRequest request)
        {
            // Generate the target URI
            Uri targetAddress = GenerateTargetUri(request);

            // Build the request and send it
            var client = this.WebClientfactory.Generate();
            NameValueCollection formData = new NameValueCollection();
            formData.Add("action", action);
            formData.Add("message", request.ToString());
            FireRequestSending(action, request);
            string response = Encoding.UTF8.GetString(client.UploadValues(targetAddress, "POST", formData));

            // Convert the response into a response object
            Response result = XmlConversionUtil.ProcessResponse(response);
            FireResponseReceived(action, result);
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
        public virtual void SendMessageAsync(string action, ServerRequest request, object userState)
        {
            // Ensure that async is only called once (or once with the user state)
            lock (lockObject)
            {
                if (userState == null)
                {
                    if (isBusy) throw new InvalidOperationException();
                    isBusy = true;
                }
                else if (asyncOperations.ContainsKey(userState))
                {
                    if (asyncOperations.ContainsKey(userState)) throw new ArgumentException("Duplicate userState", "userState");
                }
            }

            // Initialise the web client
            var client = this.WebClientfactory.Generate();
            client.UploadValuesCompleted += delegate(object sender, UploadValuesCompletedEventArgs e)
            {
                if (SendMessageCompleted != null)
                {
                    if ((e.Error != null) && !e.Cancelled)
                    {
                        // Convert the response into a response object
                        string response = Encoding.UTF8.GetString(e.Result);
                        Response result = XmlConversionUtil.ProcessResponse(response);

                        MessageReceivedEventArgs args = new MessageReceivedEventArgs(result, null, false, userState);
                        SendMessageCompleted(this, args);
                    }
                    else
                    {
                        MessageReceivedEventArgs args = new MessageReceivedEventArgs(null, e.Error, e.Cancelled, userState);
                        SendMessageCompleted(this, args);
                    }
                }
                CompleteAsyncCall(userState);
            };
            lock (lockObject)
            {
                asyncOperations.Add(userState ?? string.Empty, client);
            }

            try
            {
                // Generate the target URI
                Uri targetAddress = GenerateTargetUri(request);

                // Build the request and send it
                NameValueCollection formData = new NameValueCollection();
                formData.Add("action", action);
                formData.Add("message", request.ToString());
                client.UploadValuesAsync(targetAddress, "POST", formData);
            }
            catch (Exception error)
            {
                if (SendMessageCompleted != null)
                {
                    MessageReceivedEventArgs args = new MessageReceivedEventArgs(null, error, false, userState);
                    SendMessageCompleted(this, args);
                }
                CompleteAsyncCall(userState);
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
            lock (lockObject)
            {
                if (asyncOperations.ContainsKey(userState ?? string.Empty))
                {
                    asyncOperations[userState ?? string.Empty].CancelAsync();
                }
            }
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Disposes the .NET remoting client.
        /// </summary>
        public virtual void Dispose()
        {
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
        #region CompleteAsyncCall()
        /// <summary>
        /// Tidies up from an asynchronous call.
        /// </summary>
        /// <param name="userState">The user state that was passed in.</param>
        private void CompleteAsyncCall(object userState)
        {
            lock (lockObject)
            {
                if (userState == null)
                {
                    isBusy = false;
                }
                if (asyncOperations.ContainsKey(userState ?? string.Empty)) asyncOperations.Remove(userState ?? string.Empty);
            }
        }
        #endregion

        #region GenerateTargetUri()
        /// <summary>
        /// Generates the target URI.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Uri GenerateTargetUri(ServerRequest request)
        {
            Uri targetAddress =
                new Uri(string.Concat(serverAddress.AbsoluteUri, "/server/", request.ServerName, "/RawXmlMessage.aspx"));
            return targetAddress;
        }
        #endregion
        #endregion
    }
}
