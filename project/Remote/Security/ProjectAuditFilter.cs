using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Security
{
    /// <summary>
    /// Filters by a project name.
    /// </summary>
    [Serializable]
    public class ProjectAuditFilter
        : AuditFilterBase
    {
        private string project;

        /// <summary>
        /// Initialises a new <see cref="ProjectAuditFilter"/>.
        /// </summary>
        public ProjectAuditFilter()
        {
        }

        /// <summary>
        /// Starts a new filter with the project name.
        /// </summary>
        /// <param name="projectName"></param>
        public ProjectAuditFilter(string projectName)
            : this(projectName, null) { }

        #region Public properties
        #region ProjectName
        /// <summary>
        /// The name of the project.
        /// </summary>
        [XmlAttribute("project")]
        public string ProjectName
        {
            get { return project; }
            set { project = value; }
        }
        #endregion
        #endregion

        /// <summary>
        /// Starts a new filter with the project name and inner filter..
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="innerFilter"></param>
        public ProjectAuditFilter(string projectName, AuditFilterBase innerFilter)
            : base(innerFilter)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");
            this.project = projectName;
        }

        /// <summary>
        /// Checks if the project name matches.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override bool DoCheckFilter(AuditRecord record)
        {
            bool include = string.Equals(this.project, record.ProjectName, StringComparison.CurrentCulture);
            return include;
        }
    }
}
