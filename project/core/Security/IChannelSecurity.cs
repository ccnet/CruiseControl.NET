namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// The security requirements for sending or receiving messages.
    /// </summary>
    /// <title>Channel Security</title>
    public interface IChannelSecurity
    {
        #region Methods
        #region Validate()
        /// <summary>
        /// Validates the channel information.
        /// </summary>
        /// <param name="channelInformation"></param>
        void Validate(object channelInformation);
        #endregion
        #endregion
    }
}
