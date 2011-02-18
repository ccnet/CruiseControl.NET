namespace CruiseControl.Common
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The result of a remote action invocation.
    /// </summary>
    [DataContract]
    public class InvokeResult
    {
        #region Public properties
        #region ResultCode
        /// <summary>
        /// Gets or sets the result code.
        /// </summary>
        /// <value>
        /// The result code.
        /// </value>
        [DataMember]
        public RemoteResultCode ResultCode { get; set; }
        #endregion

        #region Data
        /// <summary>
        /// Gets or sets the data from the action.
        /// </summary>
        /// <value>
        /// The result data.
        /// </value>
        [DataMember]
        public string Data { get; set; }
        #endregion
        #endregion
    }
}