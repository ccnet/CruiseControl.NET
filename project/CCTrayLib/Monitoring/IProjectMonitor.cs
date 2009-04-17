using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Represents the current status of a project, updated only when 
	/// requested by calling Poll.
	/// 
	/// Tracks build transitions and fires events when significant changes occur.
	/// </summary>
	public interface IProjectMonitor : IPollable
	{
		ProjectState ProjectState { get; }
		IntegrationStatus IntegrationStatus { get; }
		ISingleProjectDetail Detail { get; }
		string SummaryStatusString { get; }
		string ProjectIntegratorState { get;}
		bool IsPending { get; }
		bool IsConnected { get;}

		event MonitorBuildOccurredEventHandler BuildOccurred;
		event MonitorPolledEventHandler Polled;
		event MessageEventHandler MessageReceived;

		void ForceBuild();
		void AbortBuild();
		void FixBuild(string fixingUserName);
		void StopProject();
		void StartProject();
		void CancelPending();

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        ProjectStatusSnapshot RetrieveSnapshot();
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList();
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="source">Where to retrieve the file from.</param>
        IFileTransfer RetrieveFileTransfer(string fileName, FileTransferSource source);
        #endregion
	}
}
