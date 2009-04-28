using System;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
		private readonly IWebRetriever webRetriever;
		private readonly ICruiseServerManager serverManager;
		private Uri dashboardUri;
		private Uri webUrl;
		private string serverAlias = "local";

		public HttpCruiseProjectManager(IWebRetriever webRetriever, string projectName, ICruiseServerManager serverManager)
		{
			this.projectName = projectName;
			this.webRetriever = webRetriever;
			this.serverManager = serverManager;
		}

        public void ForceBuild(string sessionToken)
		{
			PushDashboardButton(sessionToken, "ForceBuild");
		}

        public void AbortBuild(string sessionToken)
		{
			PushDashboardButton(sessionToken, "AbortBuild");
		}

		public void FixBuild(string sessionToken, string fixingUserName)
		{
			throw new NotImplementedException("Fix build not currently supported on projects monitored via HTTP");
		}

        public void StopProject(string sessionToken)
		{
			PushDashboardButton(sessionToken, "StopBuild");
		}

        public void StartProject(string sessionToken)
		{
			PushDashboardButton(sessionToken, "StartBuild");
		}

        public void CancelPendingRequest(string sessionToken)
		{
			throw new NotImplementedException("Cancel pending not currently supported on projects monitored via HTTP");
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public void PushDashboardButton(string sessionToken, string buttonName)
		{
			try
			{
				InitConnection();
				NameValueCollection input = new NameValueCollection();
				input.Add(buttonName, "true");
				input.Add("projectName", projectName);
				input.Add("serverName", serverAlias);
                input.Add("sessionToken", sessionToken);
                string response = webRetriever.Post(dashboardUri, input);

                // The dashboard catches and handles all exceptions, these exceptions need to be passed on
                HandleResponseAndThrowExceptions(response);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.WebException)
			{
			}
            // Catch any permission denied exceptions and pass in the correct permission
            catch (PermissionDeniedException)
            {
                throw new PermissionDeniedException(buttonName);
            }
		}

        private void HandleResponseAndThrowExceptions(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                if (response.Contains("ThoughtWorks.CruiseControl.Core.SessionInvalidException"))
                {
                    throw new SessionInvalidException();
                }
                else if (response.Contains("ThoughtWorks.CruiseControl.Core.PermissionDeniedException"))
                {
                    throw new PermissionDeniedException("Unknown");
                }
                else if (response.Contains("ThoughtWorks.CruiseControl.Core.SecurityException"))
                {
                    throw new SecurityException();
                }
                else if (response.Contains("ThoughtWorks.CruiseControl.Core.NoSuchProjectException"))
                {
                    throw new NoSuchProjectException();
                }
            }
		}

		private void InitConnection()
		{
			ProjectStatus ps = serverManager.GetCruiseServerSnapshot().GetProjectStatus(projectName);
			if (ps != null)
			{
				webUrl = new Uri(ps.WebURL);
				ExtractServerAlias();
			}
            dashboardUri = new Uri(new WebDashboardUrl(serverManager.Configuration.Url, serverAlias).ViewFarmReport);
		}

		private void ExtractServerAlias()
		{
			string[] splitPath = new string[0];

			if (webUrl != null)
				splitPath = webUrl.AbsolutePath.Trim('/').Split('/');

			for (int i = 0; i < splitPath.Length; i++)
			{
				if ((splitPath[i] == "server") && (splitPath[i + 1] != null) && (splitPath[i + 1] != string.Empty))
				{
					serverAlias = splitPath[i + 1];
					break;
				}
			}
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
	}
}
