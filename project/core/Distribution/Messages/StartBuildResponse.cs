namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;

    /// <summary>
    /// The response to a start build request.
    /// </summary>
    [MessageContract]
    public class StartBuildResponse
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
