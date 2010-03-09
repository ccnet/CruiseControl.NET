using System;
using System.Collections.Generic;
using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
    /// <summary>
    /// Defines a XSL file to be included in the build report.
    /// </summary>
    public class BuildReportXslFilename
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildReportXslFilename"/> class.
        /// </summary>
        public BuildReportXslFilename()
        {
            this.IncludedProjects = new List<string>();
            this.ExcludedProjects = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildReportXslFilename"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public BuildReportXslFilename(string fileName)
            : this()
        {
            this.Filename = fileName;
        }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets the included projects.
        /// </summary>
        /// <value>The included projects.</value>
        public ICollection<string> IncludedProjects { get; private set; }

        /// <summary>
        /// Gets the excluded projects.
        /// </summary>
        /// <value>The excluded projects.</value>
        public ICollection<string> ExcludedProjects { get; private set; }

        /// <summary>
        /// Checks the project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns><c>true</c> if the file is valid for the project; otherwise <c>false</c>.</returns>
        public bool CheckProject(string projectName)
        {
            if (this.IncludedProjects.Count > 0)
            {
                return this.IncludedProjects.Contains(projectName);
            }
            else if (this.ExcludedProjects.Count > 0)
            {
                return !this.ExcludedProjects.Contains(projectName);
            }
            else
            {
                return true;
            }
        }
    }
}
