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
				return new SiteTemplateResults(false, new HtmlAnchor[0], "", "", "", new HtmlAnchor[0], new HtmlAnchor[0]);
			}

			build = buildRetrieverForRequest.GetBuild(requestWrapper);
			BuildStats stats = GenerateBuildStats(build);

			return new SiteTemplateResults(true, buildLister.GetBuildLinks(serverName, projectName), stats.Html, stats.Htmlclass, "", 
				BuildPluginLinks(), ServerPluginLinks());	
		}

		private HtmlAnchor[] ServerPluginLinks()
		{
			return PluginLinks(typeof(IServerPlugin));
		}

		private HtmlAnchor[] BuildPluginLinks()
		{
			// ToDo - this better!
			ArrayList list = new ArrayList();
			list.AddRange(PluginLinks(typeof(IProjectPlugin)));
			list.AddRange(PluginLinks(typeof(IBuildPlugin)));
			return (HtmlAnchor[]) list.ToArray (typeof (HtmlAnchor));
		}

		private HtmlAnchor[] PluginLinks(Type pluginClassification)
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
					if (pluginClassification.IsAssignableFrom(plugin.GetType()))
					{
						HtmlAnchor anchor = new HtmlAnchor();

						// ToDo - this is nasty - we need a Pico like think to construct things, not just use the activator
						// ToDo if we do keep this, then test.
						if (plugin is IBuildNameRetrieverSettable)
						{
							((IBuildNameRetrieverSettable) plugin).BuildNameRetriever = buildNameRetriever;
						}

						// ToDo - clean this up
						if (pluginClassification == typeof(IServerPlugin))
						{
							anchor.HRef = ((IServerPlugin) plugin).CreateURL(build.ServerName, new DefaultServerUrlGenerator());
						}
						else if (pluginClassification == typeof(IProjectPlugin))
						{
							anchor.HRef = ((IProjectPlugin) plugin).CreateURL(build.ServerName, build.ProjectName, new DefaultProjectUrlGenerator());
						}
						else if (pluginClassification == typeof(IBuildPlugin))
						{
							anchor.HRef = ((IBuildPlugin) plugin).CreateURL(build.ServerName, build.ProjectName, build.Name, new DefaultBuildUrlGenerator());
						}
					 
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
	}
}
