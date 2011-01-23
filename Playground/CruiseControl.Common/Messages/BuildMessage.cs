namespace CruiseControl.Common.Messages
{
    using System.Runtime.Serialization;

    /// <summary>
    /// A message for a build-related action.
    /// </summary>
    [DataContract]
    public class BuildMessage
        : ProjectMessage
    {
        #region Public properties
        #region BuildName
        /// <summary>
        /// Gets or sets the name of the build.
        /// </summary>
        /// <value>
        /// The name of the build.
        /// </value>
        [DataMember]
        public string BuildName { get; set; }
        #endregion
        #endregion
    }
}
