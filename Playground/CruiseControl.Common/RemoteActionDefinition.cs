namespace CruiseControl.Common
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The definition of an action.
    /// </summary>
    [DataContract]
    public class RemoteActionDefinition
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        [DataMember]
        public string Name { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description of the action.
        /// </value>
        [DataMember]
        public string Description { get; set; }
        #endregion

        #region InputData
        /// <summary>
        /// Gets or sets the input data definition.
        /// </summary>
        /// <value>
        /// The input data definition.
        /// </value>
        [DataMember]
        public string InputData { get; set; }
        #endregion

        #region OutputData
        /// <summary>
        /// Gets or sets the output data definition.
        /// </summary>
        /// <value>
        /// The output data definition.
        /// </value>
        [DataMember]
        public string OutputData { get; set; }
        #endregion
        #endregion
    }
}