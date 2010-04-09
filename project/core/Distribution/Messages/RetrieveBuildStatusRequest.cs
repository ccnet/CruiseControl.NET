namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;

    /// <summary>
    /// A request to retrieve the status of a build.
    /// </summary>
    [MessageContract]
    public class RetrieveBuildStatusRequest
    {
        #region Public properties
        #region BuildIdentifier
        /// <summary>
        /// Gets or sets the build identifier.
        /// </summary>
        /// <value>The build identifier.</value>
        [MessageBodyMember]
        public string BuildIdentifier { get; set; }
        #endregion
        #endregion
    }
}
