using System;

namespace ThoughtWorks.CruiseControl.Core.State
{
    /// <summary>
    /// Interface to allow persisting stops for a project.
    /// </summary>
    public interface IProjectStateManager
    {
        /// <summary>
        /// Records a project as stopped.
        /// </summary>
        /// <param name="projectName">The name of the project to record</param>
        void RecordProjectAsStopped(string projectName);

        /// <summary>
        /// Records a project as being able to start automatically.
        /// </summary>
        /// <param name="projectName">The name of the project to record</param>
        void RecordProjectAsStartable(string projectName);

        /// <summary>
        /// Checks if a project can be started automatically.
        /// </summary>
        /// <param name="projectName">The name of the project to check.</param>
        /// <returns></returns>
        bool CheckIfProjectCanStart(string projectName);
    }
}
