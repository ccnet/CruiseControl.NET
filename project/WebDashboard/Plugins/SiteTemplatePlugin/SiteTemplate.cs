using System;
using System.Collections;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.config;
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
		private Build build;

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
				return new SiteTemplateResults(false, new HtmlAnchor[0], "", "", "", "", "", "", new HtmlAnchor[0], new HtmlAnchor[0]);
			}

			build = buildRetrieverForRequest.GetBuild(requestWrapper);
			BuildStats stats = GenerateBuildStats(build);
			LatestNextPreviousLinks latestNextPreviousLinks = GenerateLatestNextPreviousLinks(build);

			return new SiteTemplateResults(true, buildLister.GetBuildLinks(serverName, projectName), stats.Html, stats.Htmlclass, "", 
				latestNextPreviousLinks.latestLink, latestNextPreviousLinks.nextLink, latestNextPreviousLinks.previousLink, BuildPluginLinks(), ServerPluginLinks());	
		}

		private HtmlAnchor[] ServerPluginLinks()
		{
			return PluginLinks(PluginBehavior.Server);
		}

		private HtmlAnchor[] BuildPluginLinks ()
		{
			return PluginLinks(PluginBehavior.Build);
		}

		private HtmlAnchor[] PluginLinks(PluginBehavior behavior)
		{
			IPluginSpecification[] pluginSpecifications = configurationGetter.GetConfigFromSection(PluginsSectionHandler.SectionName) as IPluginSpecification[];
			if (pluginSpecifications == null)
			{
				// ToDo - put warning somewhere about misconfiguration
				return new HtmlAnchor[0];
			}

			ArrayList pluginAnchors = new ArrayList();
			foreach (IPluginSpecification pluginSpecification in pluginSpecifications)
			{
				Type pluginType = pluginSpecification.Type;
				if (pluginType == null)
				{
					throw new CruiseControlException("unable to create type object for typename " + pluginSpecification.TypeName);
				}
				object tempPlugin = Activator.CreateInstance(pluginSpecification.Type);
				if (tempPlugin is IPlugin)
				{
					IPlugin plugin = (IPlugin) tempPlugin;
					if (plugin.Behavior == behavior)
					{
						HtmlAnchor anchor = new HtmlAnchor();
						// ToDo - make a URL generator
						anchor.HRef = string.Format("{0}?server={1}&amp;project={2}&amp;build={3}", plugin.Url, build.ServerName, build.ProjectName, build.Name);
						anchor.InnerHtml = plugin.Description;
						anchor.Attributes["class"] = "link";
						pluginAnchors.Add(anchor);
					}
				}
				else
				{
					// ToDo - something better here
					throw new CruiseControlException(string.Format("The specified plugin {0} does not implement IPlugin", pluginSpecification.TypeName));
				}
			}

			return (HtmlAnchor[]) pluginAnchors.ToArray(typeof (HtmlAnchor));
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
				QueryStringRequestWrapper.BuildQueryStringParameter,
				buildName);
		}
	}
}
