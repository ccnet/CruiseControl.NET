using System;
namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    public interface IBuildMachine
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets the name of the machine.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the machine.
        /// </summary>
        void Initialise();
        #endregion

        #region Terminate()
        /// <summary>
        /// Terminates the machine.
        /// </summary>
        void Terminate();
        #endregion

        #region CanBuild()
        /// <summary>
        /// Determines whether this machine can build the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// <c>true</c> if this instance can build the specified project; otherwise, <c>false</c>.
        /// </returns>
        bool CanBuild(IProject project);
        #endregion

        #region StartBuild()
        /// <summary>
        /// Starts a build for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="result">The result.</param>
        /// <param name="buildCompleted">Any processing to perform when the build has completed.</param>
        /// <returns>The request for the build.</returns>
        RemoteBuildRequest StartBuild(
            IProject project, 
            IIntegrationResult result,
            Action<RemoteBuildRequest> buildCompleted);
        #endregion

        #region CancelBuild()
        /// <summary>
        /// Cancels a build.
        /// </summary>
        /// <param name="identifier">The identifier of the build.</param>
        void CancelBuild(string identifier);
        #endregion
        #endregion
    }
}
