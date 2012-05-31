namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Collections.Specialized;
    using System.Net;

    /// <summary>
    /// Default implementation of <see cref="IWebClient"/>.
    /// </summary>
    public class DefaultWebClient
        : IWebClient
    {
        #region Private fields
        private WebClient innerClient;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebClient"/> class
        /// </summary>
        public DefaultWebClient() : this(new WebClient())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebClient"/> class.
        /// </summary>
        /// <param name="webClient">instance of <see cref="WebClient"/> to use</param>
        public DefaultWebClient(WebClient webClient)
        {
            this.innerClient = webClient;
            this.innerClient.UploadValuesCompleted += (o, e) =>
            {
                // Pass on the event
                if (this.UploadValuesCompleted != null)
                {
                    this.UploadValuesCompleted(this, new BinaryDataEventArgs(e.Result, e.Error, e.Cancelled, e.UserState));
                }
            };
        }
        #endregion

        #region Public methods
        #region UploadValues()
        /// <summary>
        /// Uploads the values to the server.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        /// <returns>The response data.</returns>
        public byte[] UploadValues(Uri address, string method, NameValueCollection data)
        {
            SetCredentials(address);
            return this.innerClient.UploadValues(address, method, data);
        }

        #endregion

        #region
        /// <summary>
        /// Uploads the values to the server asynchronously.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        public void UploadValuesAsync(Uri address, string method, NameValueCollection data)
        {
            SetCredentials(address);
            this.innerClient.UploadValuesAsync(address, method, data);
        }
        #endregion

        #region CancelAsync()
        /// <summary>
        /// Cancels the async.
        /// </summary>
        public void CancelAsync()
        {
            this.innerClient.CancelAsync();
        }
        #endregion
        #endregion

        #region Public events
        #region UploadValuesCompleted
        /// <summary>
        /// Occurs when the values have completed uploading.
        /// </summary>
        public event EventHandler<BinaryDataEventArgs> UploadValuesCompleted;
        #endregion
        #endregion

        #region Private methods
        #region SetCredentials
        /// <summary>
        /// Sets credentials on client if address contains user info.
        /// </summary>
        /// <param name="address">The address to check for user info.</param>
        private void SetCredentials(Uri address)
        {
            if (address.UserInfo.Length <= 0) return;

            var userInfoValues = address.UserInfo.Split(':');
            var credentials = new NetworkCredential
                                  {
                                      UserName = userInfoValues[0]
                                  };

            if (userInfoValues.Length > 1)
                credentials.Password = userInfoValues[1];

            this.innerClient.Credentials = credentials;
        }
        #endregion
        #endregion
    }
}
