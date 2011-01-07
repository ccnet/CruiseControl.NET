namespace CruiseControl.Core.Interfaces
{
    using System.Xml;

    /// <summary>
    /// Factory methods for working with task executions.
    /// </summary>
    public interface ITaskExecutionFactory
    {
        #region Public methods
        #region StartNew()
        /// <summary>
        /// Starts a new <see cref="TaskExecutionContext"/>.
        /// </summary>
        /// <param name="logFile">The path to the log file.</param>
        /// <param name="project">The project.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="TaskExecutionContext"/>.
        /// </returns>
        TaskExecutionContext StartNew(string logFile, Project project, IntegrationRequest request);
        #endregion

        #region GenerateLogName()
        /// <summary>
        /// Generates a new log name for a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// The name of the new log for the project.
        /// </returns>
        string GenerateLogName(Project project);
        #endregion
        #endregion
    }
}
