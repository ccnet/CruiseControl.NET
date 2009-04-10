using System;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// Event args for allowing a project-based event to be canceled.
    /// </summary>
    /// <typeparam name="TData">The type of extra data to pass.</typeparam>
    public class CancelProjectEventArgs<TData>
        : ProjectEventArgs<TData>
    {
        private bool isCanceled = false;

        /// <summary>
        /// Start a new set of event args for a project with data.
        /// </summary>
        /// <param name="projectName">The name of the project this event is for.</param>
        /// <param name="data">The data to pass.</param>
        public CancelProjectEventArgs(string projectName, TData data)
            : base(projectName, data)
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
