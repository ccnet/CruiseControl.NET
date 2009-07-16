using System;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// Arguments for a project change event.
    /// </summary>
    public class ProjectChangedArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="ProjectChangedArgs"/>.
        /// </summary>
        /// <param name="project">The project that changed.</param>
        public ProjectChangedArgs(Project project)
        {
            Project = project;
        }
        #endregion

        #region Public properties
        #region Project
        /// <summary>
        /// The project that has been changed.
        /// </summary>
        public Project Project { get; protected set; }
        #endregion
        #endregion
    }
}
