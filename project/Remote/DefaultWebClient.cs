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
        /// Initializes a new instance of the <see cref="DefaultWebClient"/> class.
        /// </summary>
        public DefaultWebClient()
        {
            this.innerClient = new WebClient();
            this.innerClient.UploadValuesCompleted += (o, e) =>
            {
                // Pass on the event
                if (this.UploadValuesCompleted != null)
                {
                    this.UploadValuesCompleted(this, e);
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
        public event UploadValuesCompletedEventHandler UploadValuesCompleted;
        #endregion
        #endregion
    }
}
