namespace ThoughtWorks.CruiseControl.Core.Distribution.Messages
{
    using System.ServiceModel;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

    /// <summary>
    /// A request to start a build.
    /// </summary>
    [MessageContract]
    public class StartBuildRequest
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

        #region ProjectDefinition
        /// <summary>
        /// Gets or sets the project definition.
        /// </summary>
        /// <value>The project definition.</value>
        [MessageBodyMember]
        public string ProjectDefinition { get; set; }
        #endregion

        #region BuildCondition
        /// <summary>
        /// Gets or sets the build condition.
        /// </summary>
        /// <value>The build condition.</value>
        [MessageBodyMember]
        public BuildCondition BuildCondition { get; set; }
        #endregion

        #region Source
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [MessageBodyMember]
        public string Source { get; set; }
        #endregion

        #region UserName
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [MessageBodyMember]
        public string UserName { get; set; }
        #endregion

        #region 
        /// <summary>
        /// Gets or sets the build values.
        /// </summary>
        /// <value>The build values.</value>
        [MessageBodyMember]
        public Dictionary<string, string> BuildValues { get; set; }
        #endregion
        #endregion
    }
}
