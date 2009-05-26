using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A message for requesting an item from a project.
    /// </summary>
    [XmlRoot("projectItemMessage")]
    [Serializable]
    public class ProjectItemRequest
        : ProjectRequest
    {
        #region Constructors
        /// <summary>
        /// Initialise a new empty <see cref="ProjectItemRequest"/>.
        /// </summary>
        public ProjectItemRequest()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ProjectItemRequest"/> with a session token.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        public ProjectItemRequest(string sessionToken)
            : base(sessionToken)
        {
        }

        /// <summary>
        /// Initialise a new <see cref="ProjectItemRequest"/> with a session token and project name.
        /// </summary>
        /// <param name="sessionToken">The session token to use.</param>
        /// <param name="projectName">The name of the project.</param>
        public ProjectItemRequest(string sessionToken, string projectName)
            : base(sessionToken, projectName)
        {
        }
        #endregion

        #region Public properties
        #region BuildCondition
        /// <summary>
        /// The name of the item.
        /// </summary>
        [XmlAttribute("itemName")]
        public string ItemName { get;set;}
        #endregion
        #endregion
    }
}
