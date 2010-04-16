namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Factory for generating web clients.
    /// </summary>
    public interface IWebClientFactory
    {
        #region Public methods
        #region Generate()
        /// <summary>
        /// Generates a new client.
        /// </summary>
        /// <returns>The new client.</returns>
        IWebClient Generate();
        #endregion
        #endregion
    }
}
