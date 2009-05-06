using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A request to list the builds for a project.
    /// </summary>
    [XmlRoot("buildListMessage")]
    [Serializable]
    public class BuildListRequest
        : ProjectRequest
    {
        #region Private fields
        private int numberOfBuilds;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="BuildListRequest"/>.
        /// </summary>
        public BuildListRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildListRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public BuildListRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildListRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public BuildListRequest(string sessionToken, string projectName)
            : base(sessionToken, projectName)
        {
        }
        #endregion

        #region Public properties
        #region NumberOfBuilds
        /// <summary>
        /// The number of builds to list.
        /// </summary>
        [XmlAttribute("number")]
        public int NumberOfBuilds
        {
            get { return numberOfBuilds; }
            set { numberOfBuilds = value; }
        }
        #endregion
        #endregion
    }
}
