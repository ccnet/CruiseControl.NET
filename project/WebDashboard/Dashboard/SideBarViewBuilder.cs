using System;
using System.Collections;
using System.Web;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class SideBarViewBuilder : IConditionalGetFingerprintProvider
	{
		private readonly ICruiseRequest request;
		private readonly IBuildNameRetriever buildNameRetriever;
		private readonly IRecentBuildsViewBuilder recentBuildsViewBuilder;
		private readonly IPluginLinkCalculator pluginLinkCalculator;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ILinkListFactory linkListFactory;
		private readonly ILinkFactory linkFactory;
		private readonly IFarmService farmService;
	    private readonly IFingerprintFactory fingerprintFactory;


	    public SideBarViewBuilder(ICruiseRequest request, IBuildNameRetriever buildNameRetriever, IRecentBuildsViewBuilder recentBuildsViewBuilder, IPluginLinkCalculator pluginLinkCalculator, IVelocityViewGenerator velocityViewGenerator, ILinkFactory linkFactory, ILinkListFactory linkListFactory, IFarmService farmService, IFingerprintFactory fingerprintFactory)
		{
			this.request = request;
			this.buildNameRetriever = buildNameRetriever;
			this.recentBuildsViewBuilder = recentBuildsViewBuilder;
			this.pluginLinkCalculator = pluginLinkCalculator;
			this.velocityViewGenerator = velocityViewGenerator;
			this.linkListFactory = linkListFactory;
			this.linkFactory = linkFactory;
			this.farmService = farmService;
            this.fingerprintFactory = fingerprintFactory;
		}

		public HtmlFragmentResponse Execute()
		{
			Hashtable velocityContext = new Hashtable();
			string velocityTemplateName;

			string serverName = request.ServerName;
			if (serverName == "")
			{
				velocityContext["links"] = pluginLinkCalculator.GetFarmPluginLinks();
				velocityContext["serverlinks"] = linkListFactory.CreateServerLinkList(farmService.GetServerSpecifiers(), "ViewServerReport");
				velocityTemplateName = @"FarmSideBar.vm";
			}
			else
			{
				string projectName = request.ProjectName;
				if (projectName == "")
				{
					velocityContext["links"] = pluginLinkCalculator.GetServerPluginLinks(request.ServerSpecifier);
					velocityTemplateName = @"ServerSideBar.vm";
				}
				else
				{
					string buildName = request.BuildName;
					if (buildName == "")
					{
						IProjectSpecifier projectSpecifier = request.ProjectSpecifier;
						velocityContext["links"] = pluginLinkCalculator.GetProjectPluginLinks(projectSpecifier);
						velocityContext["recentBuildsTable"] = recentBuildsViewBuilder.BuildRecentBuildsTable(projectSpecifier);
						velocityTemplateName = @"ProjectSideBar.vm";
					}
					else
					{
						IBuildSpecifier buildSpecifier = request.BuildSpecifier;
						velocityContext["links"] = pluginLinkCalculator.GetBuildPluginLinks(buildSpecifier);
						velocityContext["recentBuildsTable"] = recentBuildsViewBuilder.BuildRecentBuildsTable(buildSpecifier);
						velocityContext["latestLink"] = linkFactory.CreateProjectLink(request.ProjectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);
						velocityContext["nextLink"] = linkFactory.CreateBuildLink(buildNameRetriever.GetNextBuildSpecifier(buildSpecifier), "", BuildReportBuildPlugin.ACTION_NAME);
						velocityContext["previousLink"] = linkFactory.CreateBuildLink(buildNameRetriever.GetPreviousBuildSpecifier(buildSpecifier), "", BuildReportBuildPlugin.ACTION_NAME);
						velocityTemplateName = @"BuildSideBar.vm";
					}
				}
			}

			return velocityViewGenerator.GenerateView(velocityTemplateName, velocityContext);
		}

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
	        ConditionalGetFingerprint mostRecentTemplateFingerprint =
                fingerprintFactory.BuildFromFileNames(@"FarmSideBar.vm", @"ServerSideBar.vm", @"ProjectSideBar.vm", @"BuildSideBar.vm");
	        return ((IConditionalGetFingerprintProvider) recentBuildsViewBuilder).GetFingerprint(request).Combine(
	            mostRecentTemplateFingerprint);
	    }
	}
}
