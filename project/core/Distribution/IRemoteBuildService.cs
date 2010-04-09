namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    using System.ServiceModel;
    using ThoughtWorks.CruiseControl.Core.Distribution.Messages;

    /// <summary>
    /// Defines the operations that can be performed by a remote build service.
    /// </summary>
    [ServiceContract(Namespace = "http://http://thoughtworks.org/ccnet/remote/1/6")]
    public interface IRemoteBuildService
    {
        #region Public methods
        #region CheckIfBuildCanRun()
        /// <summary>
        /// Checks if a build can run.
        /// </summary>
        /// <param name="request">The request detailing the project to run.</param>
        /// <returns>The response details.</returns>
        [OperationContract]
        CheckIfBuildCanRunResponse CheckIfBuildCanRun(CheckIfBuildCanRunRequest request);
        #endregion

        #region StartBuild()
        /// <summary>
        /// Starts a build.
        /// </summary>
        /// <param name="request">The request detailing the project to run.</param>
        /// <returns>The response details.</returns>
        [OperationContract]
        StartBuildResponse StartBuild(StartBuildRequest request);
        #endregion

        #region CancelBuild()
        /// <summary>
        /// Cancels a build.
        /// </summary>
        /// <param name="request">The request detailing the build to cancel.</param>
        /// <returns>The response details.</returns>
        [OperationContract]
        CancelBuildResponse CancelBuild(CancelBuildRequest request);
        #endregion

        #region RetrieveBuildStatus()
        /// <summary>
        /// Retrieves the status of a build.
        /// </summary>
        /// <param name="request">The request detailing which build to retrieve.</param>
        /// <returns>The current status of the build.</returns>
        [OperationContract]
        RetrieveBuildStatusResponse RetrieveBuildStatus(RetrieveBuildStatusRequest request);
        #endregion
        #endregion
    }
}
