namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;

    /// <summary>
    /// The response from checking if a build can run.
    /// </summary>
    [MessageContract]
    public class CheckIfBuildCanRunResponse
    {
        #region Public properties
        #region CanBuild
        /// <summary>
        /// Gets or sets a value indicating whether this instance can build.
        /// </summary>
        /// <value><c>true</c> if this instance can build; otherwise, <c>false</c>.</value>
        [MessageBodyMember]
        public bool CanBuild { get; set; }
        #endregion
        #endregion
    }
}
