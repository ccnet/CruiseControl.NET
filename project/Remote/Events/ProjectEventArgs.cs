using System;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// Event args for a project-based event.
    /// </summary>
    public class ProjectEventArgs
        : EventArgs
    {
        private readonly string projectName;

        /// <summary>
        /// Start a new set of event args for a project.
        /// </summary>
        /// <param name="projectName">The name of the project this event is for.</param>
        public ProjectEventArgs(string projectName)
        {
            this.projectName = projectName;
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName
        {
            get { return this.projectName; }
        }
    }
}
