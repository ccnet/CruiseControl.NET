using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class BuildLogBuildPlugin : ICruiseAction, IPluginLinkRenderer, IBuildPlugin
	{
		private readonly IBuildRetriever buildRetriever;

		public BuildLogBuildPlugin(IBuildRetriever buildRetriever)
		{
			this.buildRetriever = buildRetriever;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			Build build = buildRetriever.GetBuild(cruiseRequest.ServerName, cruiseRequest.ProjectName, cruiseRequest.BuildName);
			HtmlGenericControl control = new HtmlGenericControl("div");
			string buildLogForDisplay = build.Log.Replace("<", "&lt;");
			buildLogForDisplay = buildLogForDisplay.Replace(">", "&gt;");
			control.InnerHtml = string.Format(@"<p>Click <a href=""{0}"">here</a> to open log in its own page</p><pre class=""log"">{1}</pre>", 
				build.BuildLogLocation, buildLogForDisplay);
			return control;
		}

		public string Description
		{
			get { return "View Build Log"; }
		}

		public string ActionName
		{
			get { return "ViewBuildLog"; }
		}

		public Type ActionType
		{
			get { return this.GetType(); }
		}
	}
}
