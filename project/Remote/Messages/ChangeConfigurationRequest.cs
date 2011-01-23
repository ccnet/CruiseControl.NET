using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The request to change the configuration.
    /// </summary>
    [XmlRoot("changeConfigurationRequest")]
    [Serializable]
    public class ChangeConfigurationRequest
        : ProjectRequest
    {
        #region Private fields
        private string projectDefinition;
        private bool purgeWorkingDirectory;
        private bool purgeArtifactDirectory;
        private bool purgeSourceControlEnvironment;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="ChangeConfigurationRequest"/>.
        /// </summary>
        public ChangeConfigurationRequest()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ChangeConfigurationRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public ChangeConfigurationRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ChangeConfigurationRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public ChangeConfigurationRequest(string sessionToken, string projectName)
            : base(sessionToken, projectName)
        {
        }
        #endregion

        #region Public properties
        #region ProjectDefinition
        /// <summary>
        /// The XML serialisation of the project definition.
        /// </summary>
        [XmlElement("definition")]
        public string ProjectDefinition
        {
            get { return projectDefinition; }
            set { projectDefinition = value; }
        }
        #endregion

        #region PurgeWorkingDirectory
        /// <summary>
        /// Whether to purge the working directory or not.
        /// </summary>
        [XmlAttribute("purgeWorking")]
        public bool PurgeWorkingDirectory
        {
            get { return purgeWorkingDirectory; }
            set { purgeWorkingDirectory = value; }
        }
        #endregion

        #region PurgeArtifactDirectory
        /// <summary>
        /// Whether to purge the artifact directory or not.
        /// </summary>
        [XmlAttribute("purgeArtifact")]
        public bool PurgeArtifactDirectory
        {
            get { return purgeArtifactDirectory; }
            set { purgeArtifactDirectory = value; }
        }
        #endregion

        #region PurgeSourceControlEnvironment
        /// <summary>
        /// Whether to purge the source control environment or not.
        /// </summary>
        [XmlAttribute("purgeSourceControl")]
        public bool PurgeSourceControlEnvironment
        {
            get { return purgeSourceControlEnvironment; }
            set { purgeSourceControlEnvironment = value; }
        }
        #endregion
        #endregion
    }
}
