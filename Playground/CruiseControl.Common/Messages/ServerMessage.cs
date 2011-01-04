namespace CruiseControl.Common.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A message for a server-level action.
    /// </summary>
    [DataContract]
    public class ServerMessage
        : BaseMessage
    {
        #region Public properties
        #region ServerName
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        [DataMember]
        public string ServerName { get; set; }
        #endregion
        #endregion
    }
}
