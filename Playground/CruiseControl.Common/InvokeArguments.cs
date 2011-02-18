namespace CruiseControl.Common
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The arguments for a remote action.
    /// </summary>
    [DataContract]
    public class InvokeArguments
    {
        #region Public properties
        #region Action
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [DataMember]
        public string Action { get; set; }
        #endregion

        #region Data
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [DataMember]
        public string Data { get; set; }
        #endregion
        #endregion
    }
}