using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Collections.Generic;
using System.IO;

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

		void ForceBuild(Dictionary<string, string> parameters);
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

        #region TransferFile()
        /// <summary>
        /// Transfers a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="outputStream">The output stream.</param>
        void TransferFile(string fileName, Stream outputStream);
        #endregion

        List<ParameterBase> ListBuildParameters();
    }
}
