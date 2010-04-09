namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;

    /// <summary>
    /// A request to cancel a build.
    /// </summary>
    [MessageContract]
    public class CancelBuildRequest
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
