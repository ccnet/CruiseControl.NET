using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The base level message for all project-related requests.
    /// </summary>
    [XmlRoot("projectMessage")]
    [Serializable]
    public class ProjectRequest
        : ServerRequest
    {
        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="ProjectRequest"/>.
        /// </summary>
        public ProjectRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ProjectRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public ProjectRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ProjectRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public ProjectRequest(string sessionToken, string projectName)
            : base(sessionToken)
        {
            this.ProjectName = projectName;
        }
        #endregion

        #region Public properties
        #region ProjectName
        /// <summary>
        /// The name of the project that this message is for.
        /// </summary>
        [XmlAttribute("project")]
        public string ProjectName { get; set; }
        #endregion

        #region CompressData
        /// <summary>
        /// Gets or sets a value indicating whether the data should be compressed.
        /// </summary>
        /// <value><c>true</c> if the data should be compressed; otherwise, <c>false</c>.</value>
        [XmlAttribute("compress")]
        [DefaultValue(false)]
        public bool CompressData { get; set; }
        #endregion
        #endregion
    }
}
