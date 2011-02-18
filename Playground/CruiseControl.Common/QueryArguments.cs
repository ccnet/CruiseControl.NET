namespace CruiseControl.Common
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The arguments for a query.
    /// </summary>
    [DataContract]
    public class QueryArguments
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryArguments"/> class.
        /// </summary>
        public QueryArguments()
        {
            this.DataToInclude = DataDefinitions.Both;
        }
        #endregion

        #region Public properties
        #region FilterPattern
        /// <summary>
        /// Gets or sets a REGEX pattern for filtering the actions.
        /// </summary>
        /// <value>
        /// The filter pattern.
        /// </value>
        [DataMember]
        public string FilterPattern { get; set; }
        #endregion

        #region DataToInclude
        /// <summary>
        /// Gets or sets the data to include.
        /// </summary>
        /// <value>
        /// The data to include.
        /// </value>
        [DataMember]
        public DataDefinitions DataToInclude { get; set; }
        #endregion
        #endregion
    }
}