namespace CruiseControl.Core.Interfaces
{
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
        /// <param name="project">The project.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="TaskExecutionContext"/>.
        /// </returns>
        TaskExecutionContext StartNew(Project project, IntegrationRequest request);
        #endregion
        #endregion
    }
}
