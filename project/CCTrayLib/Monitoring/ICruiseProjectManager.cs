
using ThoughtWorks.CruiseControl.Remote;
namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// This is like ICruiseManager, but relates to an individual project.
	/// In due course, it may well be that cruise exposes a per-project 
	/// interface. Till then, this allows us to write code as if it does.
	/// </summary>
	public interface ICruiseProjectManager
	{
        void ForceBuild(string sessionToken);
		void FixBuild(string sessionToken, string fixingUserName);
        void AbortBuild(string sessionToken);
        void StopProject(string sessionToken);
        void StartProject(string sessionToken);
        void CancelPendingRequest(string sessionToken);
		string ProjectName { get; }

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
