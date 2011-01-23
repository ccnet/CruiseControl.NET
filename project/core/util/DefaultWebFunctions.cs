namespace ThoughtWorks.CruiseControl.Core.Util
{
    using System.Net;

    /// <summary>
    /// Default implementation of the web functions.
    /// </summary>
    public class DefaultWebFunctions
        : IWebFunctions
    {
        #region Public methods
        #region PingUrl()
        /// <summary>
        /// Pings a URL.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        /// <c>true</c> if the URL responds; <c>false</c> otherwise.
        /// </returns>
        public bool PingUrl(string address)
        {
            try
            {
                var pinger = new WebClient();
                pinger.DownloadString(address);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region PingAndValidateHeaderValue()
        /// <summary>
        /// Pings a URL and validates a header value.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="header">The header.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the header value matches; <c>false</c> otherwise.</returns>
        public bool PingAndValidateHeaderValue(string address, string header, string value)
        {
            try
            {
                var pinger = new WebClient();
                pinger.DownloadString(address);
                return (string.Compare(pinger.ResponseHeaders[header], value, true) == 0);
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion
    }
}
