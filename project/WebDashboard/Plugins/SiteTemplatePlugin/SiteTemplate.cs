using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public class SiteTemplate
	{
		private readonly IBuildNameRetriever buildNameRetriever;
		private readonly IBuildRetrieverForRequest buildRetrieverForRequest;
		private readonly IConfigurationGetter configurationGetter;
		private readonly IBuildLister buildLister;
		private readonly ICruiseRequestWrapper requestWrapper;
		private Build build;

		public SiteTemplate(ICruiseRequestWrapper requestWrapper, IConfigurationGetter configurationGetter, IBuildLister buildLister, 
			IBuildRetrieverForRequest buildRetrieverForRequest, IBuildNameRetriever buildNameRetriever)
		{
			this.requestWrapper = requestWrapper;
			this.buildLister = buildLister;
			this.configurationGetter = configurationGetter;
			this.buildRetrieverForRequest = buildRetrieverForRequest;
			this.buildNameRetriever = buildNameRetriever;
		}

		public SiteTemplateResults Do()
		{
			string serverName = requestWrapper.GetServerName();
			string projectName = requestWrapper.GetProjectName();
			if (serverName == string.Empty || projectName == string.Empty)
			{
				return new SiteTemplateResults(new HtmlAnchor[0], "", "");
			}

			build = buildRetrieverForRequest.GetBuild(requestWrapper);
			BuildStats stats = GenerateBuildStats(build);

			return new SiteTemplateResults(buildLister.GetBuildLinks(serverName, projectName), stats.Html, stats.Htmlclass);	
		}

		private struct BuildStats
		{
			public string Html;
			public string Htmlclass;
		}

		// ToDo - untested
		private BuildStats GenerateBuildStats(Build build)
		{
			BuildStats buildStats = new BuildStats();
			
			buildStats.Html = string.Format(@"Latest Build Status: {0}<br/>Time Since Latest Build: {1}", build.IsSuccessful ? "Successful" : "Failed", TimeSinceLastBuild(build));
			buildStats.Htmlclass = build.IsSuccessful ? "buildresults-data" : "buildresults-data-failed";

			return buildStats;
		}

		private string TimeSinceLastBuild(Build build)
		{
			TimeSpan interval = 	DateTime.Now - LogFileUtil.ParseForDate(LogFileUtil.ParseForDateString(build.Name));;

			if (interval.TotalHours >= 1)
			{
				return string.Format("{0} hours", (int)interval.TotalHours);
			}
			else
			{
				return string.Format("{0} minutes", (int)interval.TotalMinutes);
			}
		}
	}
}
