namespace ThoughtWorks.CruiseControl.Core.util
{
    using System;

    /// <summary>
    /// Helper methods for checking remote server URIs.
    /// </summary>
    public static class RemoteServerUri
    {
        #region Public methods
        #region IsLocal()
        /// <summary>
        /// Determines whether the URI points to the local machine.
        /// </summary>
        /// <param name="remoteServerUri">The remote server URI.</param>
        /// <returns>
        /// <c>true</c> if the specified remote server URI is local; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLocal(string remoteServerUri)
        {
            var uri = new Uri(remoteServerUri);
            return uri.IsLoopback || 
                string.Equals(uri.Host, Environment.MachineName, StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion
        #endregion
    }
}
