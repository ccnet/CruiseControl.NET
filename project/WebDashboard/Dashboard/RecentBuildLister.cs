using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class RecentBuildLister : IRecentBuildsViewBuilder, IAllBuildsViewBuilder
	{
		private readonly IVelocityTransformer velocityTransformer;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ILinkFactory linkFactory;
		private readonly ILinkListFactory linkListFactory;
		private readonly IFarmService farmService;

		public RecentBuildLister(IFarmService farmService, IVelocityTransformer velocityTransformer, 
			IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory, ILinkListFactory linkListFactory)
		{
			this.farmService = farmService;
			this.velocityTransformer = velocityTransformer;
			this.velocityViewGenerator = viewGenerator;
			this.linkFactory = linkFactory;
			this.linkListFactory = linkListFactory;
		}

		// ToDo - use concatenatable views here, not strings
		public string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier)
		{
			Hashtable primaryContext = new Hashtable();
			Hashtable secondaryContext = new Hashtable();

			secondaryContext["links"] = linkListFactory.CreateStyledBuildLinkList(farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 10), new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME));
			primaryContext["buildRows"] = velocityTransformer.Transform(@"BuildRows.vm", secondaryContext);
			primaryContext["allBuildsLink"] = linkFactory.CreateProjectLink(projectSpecifier, "", new ActionSpecifierWithName(ViewAllBuildsAction.ACTION_NAME));

			return velocityTransformer.Transform(@"RecentBuilds.vm", primaryContext);
		}

		public IView GenerateAllBuildsView(IProjectSpecifier projectSpecifier)
		{
			Hashtable primaryContext = new Hashtable();
			Hashtable secondaryContext = new Hashtable();

			secondaryContext["links"] = linkListFactory.CreateStyledBuildLinkList(farmService.GetBuildSpecifiers(projectSpecifier), new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME));
			primaryContext["buildRows"] = velocityTransformer.Transform(@"BuildRows.vm", secondaryContext);
			primaryContext["allBuildsLink"] = linkFactory.CreateProjectLink(projectSpecifier, "", new ActionSpecifierWithName(ViewAllBuildsAction.ACTION_NAME));

			return velocityViewGenerator.GenerateView(@"RecentBuilds.vm", primaryContext);
		}
	}
}
