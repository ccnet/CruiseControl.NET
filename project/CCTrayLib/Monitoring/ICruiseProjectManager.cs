using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Collections.Generic;

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
        void ForceBuild(string sessionToken, Dictionary<string, string> parameters);
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
        IFileTransfer RetrieveFileTransfer(string fileName);
        #endregion

        /// <summary>
        /// Retrieves any build parameters.
        /// </summary>
        /// <returns></returns>
        List<ParameterBase> ListBuildParameters();
	}
}
