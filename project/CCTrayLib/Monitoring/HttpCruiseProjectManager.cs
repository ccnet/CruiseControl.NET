using System;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseProjectManager : ICruiseProjectManager
	{
		private readonly string projectName;
		private readonly Uri serverUri;
		private readonly IWebRetriever webRetriever;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpCruiseProjectManager(IWebRetriever webRetriever, IDashboardXmlParser dashboardXmlParser, 
            Uri serverUri, string projectName)
		{
			this.projectName = projectName;
			this.webRetriever = webRetriever;
			this.dashboardXmlParser = dashboardXmlParser;
			this.serverUri = serverUri;
		}

		public void ForceBuild()
		{
			Uri webUrl = null;
			
			// BUG: the projects name needs to be unique across the whole dashboard
			foreach (ProjectStatus ps in
				dashboardXmlParser.ExtractAsCruiseServerSnapshot(webRetriever.Get(serverUri)).ProjectStatuses)
			{
				if (ps.Name == projectName) webUrl = new Uri(ps.WebURL);
			}
			
			string serverName = null;
			string basePath = null;
			string[] splitUrl = null;
			
			if (webUrl != null) splitUrl = webUrl.AbsolutePath.Split('/');
			
			if(splitUrl != null)
			{
				for (int i = 0; i < splitUrl.Length; i++)
				{
					if ((splitUrl[i] == "server") && (splitUrl[i + 1] != null))
					{
						serverName = splitUrl[i + 1];
						break;
					}
					if (splitUrl[i] != "") basePath = basePath + "/" + splitUrl[i];
				}

				if (serverName != null)
				{

					Uri dashboardUri = new Uri("http://" + webUrl.Host + basePath + "/server/" + serverName + "/ViewFarmReport.aspx");
					NameValueCollection input = new NameValueCollection();
					
					input.Add("forcebuild", "true");
					input.Add("forceBuildProject", projectName);
					input.Add("forceBuildServer", serverName);
					
					webRetriever.Post(dashboardUri, input);
				}
				else
				{
				}
			}
		}

		public void FixBuild()
		{
			throw new NotImplementedException("Fix build not currently supported on projects monitored via HTTP");
		}

		public void CancelPendingRequest()
		{
			throw new NotImplementedException("Cancel pending not currently supported on projects monitored via HTTP");
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}
