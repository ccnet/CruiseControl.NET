using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The base level message for all build-related requests.
    /// </summary>
    [XmlRoot("buildMessage")]
    [Serializable]
    public class BuildRequest
        : ProjectRequest
    {
        #region Private fields
        private string buildName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="BuildRequest"/>.
        /// </summary>
        public BuildRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public BuildRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public BuildRequest(string sessionToken, string projectName)
            : base(sessionToken, projectName)
        {
        }
        #endregion

        #region Public properties
        #region BuildName
        /// <summary>
        /// The name of the build to retrieve the log for.
        /// </summary>
        [XmlAttribute("build")]
        public string BuildName
        {
            get { return buildName; }
            set { buildName = value; }
        }
        #endregion
        #endregion
    }
}
