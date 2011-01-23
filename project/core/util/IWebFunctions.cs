namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Some common web functionality.
    /// </summary>
    public interface IWebFunctions
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
        bool PingUrl(string address);
        #endregion

        #region PingAndValidateHeaderValue()
        /// <summary>
        /// Pings a URL and validates a header value.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="header">The header.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the header value matches; <c>false</c> otherwise.</returns>
        bool PingAndValidateHeaderValue(string address, string header, string value);
        #endregion
        #endregion
    }
}
