using System;
using System.Collections;
using System.Web;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	[ReflectorType("projectReportProjectPlugin")]
	public class ProjectReportProjectPlugin : ICruiseAction, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly ILinkFactory linkFactory;
		public static readonly string ACTION_NAME = "ViewProjectReport";
		private IBuildPlugin[] pluginNames = null;

		[ReflectorArray("reportPlugins", Required=false)]
		public IBuildPlugin[] DashPlugins
		{
			get { return pluginNames; }
			set { pluginNames = value; }
		}

		public ProjectReportProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
			this.linkFactory = linkFactory;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;

			IBuildSpecifier[] buildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 1);
			if (buildSpecifiers.Length == 1)
			{
				velocityContext["mostRecentBuildUrl"] = linkFactory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME).Url;
			}

			velocityContext["projectName"] = projectSpecifier.ProjectName;
			velocityContext["externalLinks"] = farmService.GetExternalLinks(projectSpecifier);
			velocityContext["noLogsAvailable"] = (buildSpecifiers.Length == 0);


            velocityContext["applicationPath"] = cruiseRequest.Request.ApplicationPath;
            velocityContext["rssDataPresent"] = farmService.GetRSSFeed(projectSpecifier).Length > 0;
            
            // I can not figure out why the lone below does not work :-(
            // velocityContext["rss"] = linkFactory.CreateProjectLink(projectSpecifier, WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME).Url;
            //
            velocityContext["rss"] = RSSLinkBuilder.CreateRSSLink(linkFactory, projectSpecifier);
            
            
			string subReportData = GetPluginSubReport(cruiseRequest, projectSpecifier, buildSpecifiers);
			if (subReportData != null && subReportData != String.Empty)
				velocityContext["pluginInfo"] = subReportData;

			return viewGenerator.GenerateView(@"ProjectReport.vm", velocityContext);
		}

		public string LinkDescription
		{
			get { return "Project Report"; }
		}

		public INamedAction[] NamedActions
		{
			get { return new INamedAction[] {new ImmutableNamedAction(ACTION_NAME, this)}; }
		}

		private string GetPluginSubReport(ICruiseRequest cruiseRequest,
		                                  IProjectSpecifier projectSpecifier, IBuildSpecifier[] buildSpecifiers)
		{
			if (buildSpecifiers.Length > 0 && pluginNames != null)
			{
				string outputResponse = String.Empty;

				ModifiedCruiseRequest req = new ModifiedCruiseRequest(cruiseRequest.Request);
				req.ReplaceBuildSpecifier(buildSpecifiers[0]);

				foreach (IBuildPlugin buildPlugIn in pluginNames)
				{
					if (buildPlugIn != null && buildPlugIn.IsDisplayedForProject(projectSpecifier) &&
					    buildPlugIn.NamedActions != null)
					{
						foreach (INamedAction namedAction in buildPlugIn.NamedActions)
						{
							IResponse resp = namedAction.Action.Execute(req);

							if (resp != null && resp is HtmlFragmentResponse)
								outputResponse += ((HtmlFragmentResponse) resp).ResponseFragment;
						}
					}
				}
				return outputResponse;
			}
			return null;
		}

		private class ModifiedCruiseRequest : ICruiseRequest
		{
			private readonly IRequest request;

			private IServerSpecifier serverSpecifier = null;
			private IProjectSpecifier projectSpecifier = null;
			private IBuildSpecifier buildSpecifier = null;

			public ModifiedCruiseRequest(IRequest request)
			{
				this.request = request;
			}

			public string ServerName
			{
				get { return (serverSpecifier != null) ? serverSpecifier.ServerName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ServerRESTSpecifier); }
			}

			public string ProjectName
			{
				get { return (projectSpecifier != null) ? projectSpecifier.ProjectName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ProjectRESTSpecifier); }
			}

			public string BuildName
			{
				get { return (buildSpecifier != null) ? buildSpecifier.BuildName : FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.BuildRESTSpecifier); }
			}

			private string FindRESTSpecifiedResource(string specifier)
			{
				string[] subFolders = request.SubFolders;

				for (int i = 0; i < subFolders.Length; i += 2)
				{
					if (subFolders[i] == specifier)
					{
						if (i < subFolders.Length)
						{
							return HttpUtility.UrlDecode(subFolders[i + 1]);
						}
						else
						{
							throw new CruiseControlException(
								string.Format("unexpected URL format - found {0} REST Specifier, but no following value", specifier));
						}
					}
				}

				return "";
			}

			public IServerSpecifier ServerSpecifier
			{
				get { return (serverSpecifier != null) ? serverSpecifier : new DefaultServerSpecifier(ServerName); }
			}

			public IProjectSpecifier ProjectSpecifier
			{
				get { return (projectSpecifier != null) ? projectSpecifier : new DefaultProjectSpecifier(ServerSpecifier, ProjectName); }
			}

			public IBuildSpecifier BuildSpecifier
			{
				get { return (buildSpecifier != null) ? buildSpecifier : new DefaultBuildSpecifier(ProjectSpecifier, BuildName); }
			}

			public IRequest Request
			{
				get { return request; }
			}

			public void ReplaceBuildSpecifier(IBuildSpecifier buildSpecifier)
			{
				this.buildSpecifier = buildSpecifier;
			}
		}
	}
}