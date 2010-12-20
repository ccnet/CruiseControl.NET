namespace CruiseControl.Core
{
    /// <summary>
    /// The state of a project.
    /// </summary>
    public enum ProjectState
    {
        /// <summary>
        /// The project is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// The project is running.
        /// </summary>
        Running,

        /// <summary>
        /// The project is starting
        /// </summary>
        Starting,

        /// <summary>
        /// The project is stopping.
        /// </summary>
        Stopping,

        /// <summary>
        /// The state of the project is unknown.
        /// </summary>
        Unknown,
    }
}
