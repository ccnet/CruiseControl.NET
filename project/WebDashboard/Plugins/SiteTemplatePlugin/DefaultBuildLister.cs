using System;
using System.Collections;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public class DefaultBuildLister : IBuildLister
	{
		private readonly ICruiseManagerWrapper manager;

		public DefaultBuildLister(ICruiseManagerWrapper manager)
		{
			this.manager = manager;
		}

		public HtmlAnchor[] GetBuildLinks(string serverName, string projectName)
		{
			if (projectName == null || projectName == string.Empty || serverName == null || serverName == string.Empty)
			{
				return new HtmlAnchor[0];
			}
			else
			{
				return GenerateBuildLinks(manager.GetBuildNames(serverName, projectName), projectName);
			}
		}

		private HtmlAnchor[] GenerateBuildLinks(string[] buildNames, string projectName)
		{
			ArrayList anchorList = new ArrayList();
			foreach (string buildName in buildNames)
			{
				bool isSuccessful = LogFileUtil.IsSuccessful(buildName);
				// To Do - get rid of these static methods - maybe use the Log Specifier class?
				HtmlAnchor anchor = new HtmlAnchor();
				anchor.Attributes["class"] =  isSuccessful ? "link" : "link-failed";
				anchor.HRef = LogFileUtil.CreateUrl(buildName, projectName);
				anchor.InnerHtml = string.Format("<nobr>{0} ({1})</nobr>", 
					LogFileUtil.GetFormattedDateString(buildName), 
					isSuccessful ?  LogFileUtil.ParseBuildNumber(buildName) : "Failed");	
				anchorList.Add(anchor);
			}

			return (HtmlAnchor[]) anchorList.ToArray(typeof (HtmlAnchor));
		}
	}
}
