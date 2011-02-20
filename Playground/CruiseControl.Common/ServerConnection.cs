namespace CruiseControl.Common
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// A connection to a server.
    /// </summary>
    public class ServerConnection
    {
        #region Public methods
        #region Ping()
        /// <summary>
        /// Attempt to ping the server.
        /// </summary>
        /// <param name="uri">The URI for the server.</param>
        /// <returns>
        /// <c>true</c> if the ping was successful; <c>false</c> otherwise.
        /// </returns>
        public static bool Ping(string uri)
        {
            var binding = new NetTcpBinding();
            var address = new EndpointAddress(uri);
            try
            {
                var channel = ChannelFactory<ICommunicationsChannel>
                    .CreateChannel(binding, address);
                try
                {
                    return channel.Ping();
                }
                finally
                {
                    var disposable = channel as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
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
