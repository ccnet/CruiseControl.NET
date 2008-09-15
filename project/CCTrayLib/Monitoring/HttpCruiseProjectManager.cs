using System;
using System.Collections.Specialized;
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

		public void ForceBuild()
		{
			PushDashboardButton("ForceBuild");
		}

		public void AbortBuild()
		{
			PushDashboardButton("AbortBuild");
		}

		public void FixBuild(string fixingUserName)
		{
			throw new NotImplementedException("Fix build not currently supported on projects monitored via HTTP");
		}

		public void StopProject()
		{
			PushDashboardButton("StopBuild");
		}

		public void StartProject()
		{
			PushDashboardButton("StartBuild");
		}

		public void CancelPendingRequest()
		{
			throw new NotImplementedException("Cancel pending not currently supported on projects monitored via HTTP");
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public void PushDashboardButton(string buttonName)
		{
			try
			{
				InitConnection();
				NameValueCollection input = new NameValueCollection();
				input.Add(buttonName, "true");
				input.Add("projectName", projectName);
				input.Add("serverName", serverAlias);
				webRetriever.Post(dashboardUri, input);
			}
			// Silently ignore exceptions that occur due to connection problems
			catch (System.Net.WebException)
			{
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
			dashboardUri = new Uri(new WebDashboardUrl(serverManager.ServerUrl, serverAlias).ViewFarmReport);
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
	}
}
