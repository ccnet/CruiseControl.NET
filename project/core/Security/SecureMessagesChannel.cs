using Exortech.NetReflector;
using System;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// Messages will be encrypted using this channel.
    /// </summary>
    /// <title>Encrypted Messages Channel</title>
    /// <version>1.5</version>
    [ReflectorType("encryptedChannel")]
    public class SecureMessagesChannel
        : IChannelSecurity
    {
        #region Methods
        #region Validate()
        /// <summary>
        /// Validates the channel information.
        /// </summary>
        /// <param name="channelInformation"></param>
        public virtual void Validate(object channelInformation)
        {
            var information = channelInformation as ChannelSecurityInformation;
            if (information == null) throw new SecurityException("Communications channel does not have any security");
            if (!information.IsEncrypted) throw new SecurityException("Message was not encrypted");
        }
        #endregion
        #endregion
    }
}
