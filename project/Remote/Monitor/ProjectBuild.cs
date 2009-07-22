using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// Details on a build for a project.
    /// </summary>
    public class ProjectBuild
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        private readonly Project project;
        private readonly string name;
        private string log;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new project build entity.
        /// </summary>
        /// <param name="buildName">The name of the build.</param>
        /// <param name="project">The project this build belongs to.</param>
        /// <param name="client">The underlying client.</param>
        public ProjectBuild(string buildName, Project project, CruiseServerClientBase client)
        {
            this.name = buildName;
            this.project = project;
            this.client = client;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the build.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        #endregion

        #region Log
        /// <summary>
        /// The log for the build.
        /// </summary>
        /// <remarks>
        /// This property uses lazy loading to retrieve the log from the server.
        /// </remarks>
        public string Log
        {
            get
            {
                if (string.IsNullOrEmpty(log))
                {
                    client.ProcessSingleAction<object>(o =>
                    {
                        log = client.GetLog(project.Name, name);
                    }, null);
                }
                return log;
            }
        }
        #endregion
        #endregion
    }
}
