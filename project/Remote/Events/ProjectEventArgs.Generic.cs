using System;

namespace ThoughtWorks.CruiseControl.Remote.Events
{
    /// <summary>
    /// Event args for a project-based event.
    /// </summary>
    /// <typeparam name="TData">The type of extra data to pass.</typeparam>
    public class ProjectEventArgs<TData>
        : EventArgs
    {
        private readonly string projectName;
        private readonly TData data;

        /// <summary>
        /// Start a new set of event args for a project with data.
        /// </summary>
        /// <param name="projectName">The name of the project this event is for.</param>
        /// <param name="data">The data to pass.</param>
        public ProjectEventArgs(string projectName, TData data)
        {
            this.projectName = projectName;
            this.data = data;
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName
        {
            get { return this.projectName; }
        }

        /// <summary>
        /// Any extra data for this event.
        /// </summary>
        public TData Data
        {
            get { return this.data; }
        }
    }
}
