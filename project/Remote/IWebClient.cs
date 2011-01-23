namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.Collections.Specialized;
    using System.Net;

    /// <summary>
    /// Abstracts web client so it can be used in unit testing.
    /// </summary>
    public interface IWebClient
    {
        #region Public methods
        #region UploadValues()
        /// <summary>
        /// Uploads the values to the server.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        /// <returns>The response data.</returns>
        byte[] UploadValues(Uri address, string method, NameValueCollection data);
        #endregion

        #region 
        /// <summary>
        /// Uploads the values to the server asynchronously.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        void UploadValuesAsync(Uri address, string method, NameValueCollection data);
        #endregion

        #region CancelAsync()
        /// <summary>
        /// Cancels the async.
        /// </summary>
        void CancelAsync();
        #endregion
        #endregion

        #region Public events
        #region UploadValuesCompleted
        /// <summary>
        /// Occurs when the values have completed uploading.
        /// </summary>
        event EventHandler<BinaryDataEventArgs> UploadValuesCompleted;
        #endregion
        #endregion
    }
}
