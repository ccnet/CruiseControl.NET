namespace CruiseControl.Common.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A message for a project level action.
    /// </summary>
    [DataContract]
    public class ProjectMessage
        : ServerMessage
    {
        #region Public properties
        #region ProjectName
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }
        #endregion
        #endregion
    }
}
