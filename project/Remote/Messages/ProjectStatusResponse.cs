using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// The response containing the status of the projects.
    /// </summary>
    [XmlRoot("projectStatusResponse")]
    [Serializable]
    public class ProjectStatusResponse
        : Response
    {
        #region Private fields
        private List<ProjectStatus> projects = new List<ProjectStatus>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new instance of <see cref="ProjectStatusResponse"/>.
        /// </summary>
        public ProjectStatusResponse()
            : base()
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ProjectStatusResponse"/> from a request.
        /// </summary>
        /// <param name="request">The request to use.</param>
        public ProjectStatusResponse(ServerRequest request)
            : base(request)
        {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="ProjectStatusResponse"/> from a response.
        /// </summary>
        /// <param name="response">The response to use.</param>
        public ProjectStatusResponse(Response response)
            : base(response)
        {
        }
        #endregion

        #region Public properties
        #region Projects
        /// <summary>
        /// The projects.
        /// </summary>
        [XmlElement("project")]
        public List<ProjectStatus> Projects
        {
            get { return projects; }
            set { projects = value; }
        }
        #endregion
        #endregion
    }
}
