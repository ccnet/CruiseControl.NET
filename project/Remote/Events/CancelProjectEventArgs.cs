using System;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// Event args for allowing a project-based event to be canceled.
    /// </summary>
    public class CancelProjectEventArgs
        : ProjectEventArgs
    {
        private bool isCanceled/* = false*/;

        /// <summary>
        /// Start a new set of event args for a project.
        /// </summary>
        /// <param name="projectName">The name of the project this event is for.</param>
        public CancelProjectEventArgs(string projectName)
            : base(projectName)
        {
        }

        /// <summary>
        /// Whether the event should be canceled or not.
        /// </summary>
        public bool Cancel
        {
            get { return this.isCanceled; }
            set { this.isCanceled = value; }
        }
    }
}
