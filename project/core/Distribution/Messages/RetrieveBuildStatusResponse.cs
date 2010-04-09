namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// The current status of a build.
    /// </summary>
    [MessageContract]
    public class RetrieveBuildStatusResponse
    {
        #region Public properties
        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [MessageBodyMember]
        public IntegrationStatus Status { get; set; }
        #endregion
        #endregion
    }
}
