using System;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Xml;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
        private readonly CruiseServerClientBase client;
		private readonly ICruiseServerManager serverManager;

		public HttpCruiseProjectManager(CruiseServerClientBase client, string projectName, ICruiseServerManager serverManager)
		{
			this.projectName = projectName;
            this.client = client;
			this.serverManager = serverManager;
		}

        public void ForceBuild(string sessionToken, Dictionary<string, string> parameters, string userName)
		{
            client.SessionToken = sessionToken;
            client.DisplayName = userName;
            client.ForceBuild(projectName, NameValuePair.FromDictionary(parameters));
		}

        public void AbortBuild(string sessionToken)
		{
            client.SessionToken = sessionToken;
            client.AbortBuild(projectName);
		}

		public void FixBuild(string sessionToken, string fixingUserName)
		{
			throw new NotImplementedException("Fix build not currently supported on projects monitored via HTTP");
		}

        public void StopProject(string sessionToken)
		{
            client.SessionToken = sessionToken;
            client.StopProject(projectName);
        }

        public void StartProject(string sessionToken)
		{
            client.SessionToken = sessionToken;
            client.StartProject(projectName);
        }

        public void CancelPendingRequest(string sessionToken)
		{
			throw new NotImplementedException("Cancel pending not currently supported on projects monitored via HTTP");
		}

		public string ProjectName
		{
			get { return projectName; }
		}

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        public virtual ProjectStatusSnapshot RetrieveSnapshot()
        {
            ProjectStatusSnapshot snapshot = new ProjectStatusSnapshot();
            snapshot.Name = projectName;
            snapshot.Status = ItemBuildStatus.Unknown;
            return snapshot;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList()
        {
            PackageDetails[] list = new PackageDetails[0];
            return list;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual IFileTransfer RetrieveFileTransfer(string fileName)
        {
            throw new InvalidOperationException();
        }
        #endregion

        /// <summary>
        /// Retrieves any build parameters.
        /// </summary>
        /// <returns></returns>
        public virtual List<ParameterBase> ListBuildParameters()
        {
            var parameters = client.ListBuildParameters(projectName);
            return parameters;
        }
	}
}
