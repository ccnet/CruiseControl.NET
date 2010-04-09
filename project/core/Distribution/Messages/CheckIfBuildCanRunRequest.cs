namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;

    /// <summary>
    /// The request for checking if a build can run.
    /// </summary>
    [MessageContract]
    public class CheckIfBuildCanRunRequest
    {
        #region Public properties
        #region ProjectName
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        [MessageBodyMember]
        public string ProjectName { get; set; }
        #endregion
        #endregion
    }
}
