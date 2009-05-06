using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A message for requesting an integration.
    /// </summary>
    [XmlRoot("integrationMessage")]
    [Serializable]
    public class BuildIntegrationRequest
        : ProjectRequest
    {
        #region Private fields
        private BuildCondition buildCondition = BuildCondition.ForceBuild;
        private List<NameValuePair> buildValues = new List<NameValuePair>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="BuildRequest"/>.
        /// </summary>
        public BuildIntegrationRequest()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public BuildIntegrationRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="BuildRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public BuildIntegrationRequest(string sessionToken, string projectName)
            : base(sessionToken, projectName)
        {
        }
        #endregion

        #region Public properties
        #region BuildCondition
        /// <summary>
        /// The type of build condition.
        /// </summary>
        [XmlAttribute("condition")]
        public BuildCondition BuildCondition
        {
            get { return buildCondition; }
            set { buildCondition = value; }
        }
        #endregion

        #region BuildValues
        /// <summary>
        /// The values to pass into the build.
        /// </summary>
        [XmlElement("buildValue")]
        public List<NameValuePair> BuildValues
        {
            get { return buildValues; }
            set { buildValues = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region AddBuildValue
        /// <summary>
        /// Adds a new build value.
        /// </summary>
        /// <param name="name">The name of the build value.</param>
        /// <param name="value">The value of the build value.</param>
        /// <returns>The new build value.</returns>
        public NameValuePair AddBuildValue(string name, string value)
        {
            NameValuePair credential = new NameValuePair(name, value);
            buildValues.Add(credential);
            return credential;
        }
        #endregion
        #endregion
    }
}
