namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    /// <summary>
    /// Defines a build agent that can build remotely.
    /// </summary>
    public interface IBuildAgent
    {
        #region Public properties
        #region ConfigurationReader
        /// <summary>
        /// Gets or sets the configuration reader.
        /// </summary>
        /// <value>The configuration reader.</value>
        INetReflectorConfigurationReader ConfigurationReader { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the agent.
        /// </summary>
        void Initialise();
        #endregion

        #region Terminate()
        /// <summary>
        /// Terminates the agent.
        /// </summary>
        void Terminate();
        #endregion
        #endregion
    }
}
