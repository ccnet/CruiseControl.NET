namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Provides the basic security information for a channel.
    /// </summary>
    public abstract class ChannelSecurityInformation
    {
        #region Public properties
        #region IsEncrypted
        /// <summary>
        /// Has the message been encrypted.
        /// </summary>
        public bool IsEncrypted { get; set; }
        #endregion
        #endregion
    }
}
