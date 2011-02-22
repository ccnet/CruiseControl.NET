namespace CruiseControl.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The result of a query.
    /// </summary>
    [DataContract]
    public class QueryResult
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult"/> class.
        /// </summary>
        public QueryResult()
        {
            this.ResultCode = RemoteResultCode.Success;
        }
        #endregion

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

        #region LogId
        /// <summary>
        /// Gets or sets the log id.
        /// </summary>
        /// <value>
        /// The log id.
        /// </value>
        [DataMember]
        public Guid? LogId { get; set; }
        #endregion

        #region Actions
        /// <summary>
        /// Gets or sets the available actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        [DataMember]
        public RemoteActionDefinition[] Actions { get; set; }
        #endregion
        #endregion
    }
}