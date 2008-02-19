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
		private readonly Uri dashboardUri;
		private Uri webUrl;
		private string serverName;
		private string basePath;

		public HttpCruiseProjectManager(IWebRetriever webRetriever, string projectName, ICruiseServerManager serverManager)
		{
			this.projectName = projectName;
			this.webRetriever = webRetriever;
			this.serverManager = serverManager;
			GetWebUrl();
			ExtractBasePathAndServerName();
			dashboardUri = new Uri(string.Format("http://{0}/{1}/server/{2}/ViewFarmReport.aspx", webUrl.Host, basePath, serverName));
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
		
		public string ProjectIntegratorState
		{
			get
			{
				return serverManager.GetCruiseServerSnapshot().GetProjectStatus(projectName).Status.ToString();
			}
		}

		public void PushDashboardButton(string buttonName)
		{
			NameValueCollection input = new NameValueCollection();
			input.Add(buttonName, "true");
			input.Add("projectName", projectName);
			input.Add("serverName", serverName);
			webRetriever.Post(dashboardUri, input);
		}

		private void GetWebUrl()
		{
			// BUG: the projects name needs to be unique across the whole dashboard
			foreach (ProjectStatus ps in serverManager.GetCruiseServerSnapshot().ProjectStatuses)
			{
				if (ps.Name == projectName)
				{
					webUrl = new Uri(ps.WebURL);
					break;
				}
			}
		}

		private void ExtractBasePathAndServerName()
		{
			string[] splitPath = webUrl.AbsolutePath.Trim('/').Split('/');
			for (int i = 0; i < splitPath.Length; i++)
			{
				if ((splitPath[i] == "server") && (splitPath[i + 1] != null))
				{
					serverName = splitPath[i + 1];
					break;
				}
				basePath = string.Format("{0}/{1}", basePath , splitPath[i]);
			}
			basePath = basePath.Trim('/');
		}
	}
}
