using System;
using System.Collections;
using System.Text;
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
		private readonly IRequestWrapper requestWrapper;

		public SiteTemplate(IRequestWrapper requestWrapper, IConfigurationGetter configurationGetter, IBuildLister buildLister, 
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
				return new SiteTemplateResults(false, null, "", "", "", "", "", "");
			}

			Build build = buildRetrieverForRequest.GetBuild(requestWrapper);
			BuildStats stats = GenerateBuildStats(build);
			LatestNextPreviousLinks latestNextPreviousLinks = GenerateLatestNextPreviousLinks(build);

			return new SiteTemplateResults(true, buildLister.GetBuildLinks(serverName, projectName), stats.Html, stats.Htmlclass, GeneratePluginLinks(build), 
				latestNextPreviousLinks.latestLink, latestNextPreviousLinks.nextLink, latestNextPreviousLinks.previousLink);	
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
			
			buildStats.Html = string.Format(@"Latest Build Status: {0}<br/>\nTime Since Latest Build: {1}", build.IsSuccessful ? "Successful" : "Failed", TimeSinceLastBuild(build));
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


		// ToDo - untested (but are we going to change how we do plugins? )
		private string GeneratePluginLinks(Build build)
		{
			string pluginLinksHtml = "";
			object pluginSpecs = configurationGetter.GetConfigFromSection("CCNet/projectPlugins");

			if (pluginSpecs != null)
			{
				foreach (PluginSpecification spec in (IEnumerable) pluginSpecs)
				{
					pluginLinksHtml += String.Format(@"|&nbsp; <a class=""link"" href=""{0}?{1}={2}"">{3}</a> ", 
						spec.LinkUrl, LogFileUtil.ProjectQueryString, build.ProjectName, spec.LinkText);
				}
			}
			return pluginLinksHtml;
		}

		private struct LatestNextPreviousLinks
		{
			public string latestLink;
			public string previousLink;
			public string nextLink;
		}

		// ToDo - untested
		private LatestNextPreviousLinks GenerateLatestNextPreviousLinks(Build build)
		{
			LatestNextPreviousLinks links = new LatestNextPreviousLinks();

			links.latestLink = ProjectReportPageWithNoBuild(build);
			links.previousLink = ProjectReportPageWithBuild(buildNameRetriever.GetPreviousBuildName(build), build);
			links.nextLink = ProjectReportPageWithBuild(buildNameRetriever.GetNextBuildName(build), build);

			return links;
		}

		// ToDo - we need a URL generator
		private string ProjectReportPageWithNoBuild(Build currentlyViewedBuild)
		{
			return "projectreport.aspx" + string.Format("?{0}={1}&{2}={3}", 
				QueryStringRequestWrapper.ServerQueryStringParameter, 
				currentlyViewedBuild.ServerName,
				QueryStringRequestWrapper.ProjectQueryStringParameter,
				currentlyViewedBuild.ProjectName);
		}

		private string ProjectReportPageWithBuild(string buildName, Build currentlyViewedBuild)
		{
			return string.Format("{0}&{1}={2}", 
				ProjectReportPageWithNoBuild(currentlyViewedBuild), 
				QueryStringRequestWrapper.LogQueryStringParameter,
				buildName);
		}
	}
}
