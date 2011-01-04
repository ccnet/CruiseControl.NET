namespace CruiseControl.Common.Messages
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The common base for all messages.
    /// </summary>
    [DataContract]
    public abstract class BaseMessage
    {
        #region Public properties
        #region Identifier
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public Guid Identifier { get; set; }
        #endregion

        #region TimeStamp
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public DateTime TimeStamp { get; set; }
        #endregion
        #endregion
    }
}
