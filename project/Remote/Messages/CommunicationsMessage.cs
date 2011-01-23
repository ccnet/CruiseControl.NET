using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The base message class for all requests and responses to inherit from.
    /// </summary>
    [Serializable]
    public abstract class CommunicationsMessage
    {
        #region Private fields
        private DateTime timestamp = DateTime.Now;
        [NonSerialized]
        private object channelInformation;
        #endregion

        #region Public properties
        #region Timestamp
        /// <summary>
        /// The timestamp of when this message was generated.
        /// </summary>
        [XmlAttribute("timestamp")]
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }
        #endregion

        #region ChannelInformation
        /// <summary>
        /// Information on what channel was used to transmit this request.
        /// </summary>
        /// <remarks>
        /// This information will typically be set by the communications channel. It will not
        /// be passed between the server and the client.
        /// </remarks>
        [XmlIgnore]
        public object ChannelInformation
        {
            get { return channelInformation; }
            set { channelInformation = value; }
        }
        #endregion
        #endregion
    }
}
