namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Generic factory for generating clients.
    /// </summary>
    /// <typeparam name="TClient">The type of the client.</typeparam>
    public class WebClientFactory<TClient>
        : IWebClientFactory
        where TClient : IWebClient, new()
    {
        #region Public methods
        #region Generate()
        /// <summary>
        /// Generates a new client.
        /// </summary>
        /// <returns>The new client.</returns>
        public IWebClient Generate()
        {
            var client = new TClient();
            return client;
        }
        #endregion
        #endregion
    }
}
