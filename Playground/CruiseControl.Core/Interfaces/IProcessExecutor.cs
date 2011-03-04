namespace CruiseControl.Core.Interfaces
{
    using System;
    using CruiseControl.Core.Utilities;

    /// <summary>
    /// An executor for external processes.
    /// </summary>
    public interface IProcessExecutor
    {
        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes the specified process.
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="outputFile">The output file.</param>
        /// <returns>
        /// The result of the execution.
        /// </returns>
        ProcessResult Execute(ProcessInfo processInfo, string projectName, string itemId, string outputFile);

        /// <summary>
        /// Executes the specified process for a project item.
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <param name="item">The item.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The result of the execution.
        /// </returns>
        ProcessResult Execute(ProcessInfo processInfo, ProjectItem item, TaskExecutionContext context);
        #endregion
        #endregion

        #region Public events
        #region ProcessOutput
        /// <summary>
        /// Occurs when process has some output.
        /// </summary>
        event EventHandler<ProcessOutputEventArgs> ProcessOutput;
        #endregion
        #endregion
    }
}