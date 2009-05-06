using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A server connection over HTTP.
    /// </summary>
    public class HttpConnection
        : IServerConnection
    {
        #region Private fields
        private readonly Uri serverAddress;
        private bool isBusy;
        private Dictionary<object, WebClient> asyncOperations = new Dictionary<object, WebClient>();
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
            Uri targetAddress = new Uri(serverAddress,
                string.Format("/server/{0}/RawXmlMessage.aspx", request.ServerName));

            // Build the request and send it
            WebClient client = new WebClient();
            NameValueCollection formData = new NameValueCollection();
            formData.Add("action", action);
            formData.Add("message", request.ToString());
            string response = Encoding.UTF8.GetString(client.UploadValues(targetAddress, "POST", formData));

            // Convert the response into a response object
            Response result = XmlConversionUtil.ProcessResponse(response);
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
            WebClient client = new WebClient();
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
                Uri targetAddress = new Uri(serverAddress,
                    string.Format("/server/{0}/RawXmlMessage.aspx", request.ServerName));

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
        #endregion
    }
}
