using System;
using System.Collections;
using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class RecentBuildLister : IRecentBuildsViewBuilder, IAllBuildsViewBuilder, IConditionalGetFingerprintProvider
	{
		private readonly IVelocityTransformer velocityTransformer;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ILinkFactory linkFactory;
		private readonly ILinkListFactory linkListFactory;
	    private readonly IFingerprintFactory fingerprintFactory;
	    private readonly IFarmService farmService;
        private readonly ICruiseUrlBuilder urlBuilder;

		public RecentBuildLister(IFarmService farmService, IVelocityTransformer velocityTransformer, 
			IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory, ILinkListFactory linkListFactory, IFingerprintFactory fingerprintFactory,
            ICruiseUrlBuilder urlBuilder)
		{
			this.farmService = farmService;
			this.velocityTransformer = velocityTransformer;
			this.velocityViewGenerator = viewGenerator;
			this.linkFactory = linkFactory;
			this.linkListFactory = linkListFactory;
            this.urlBuilder = urlBuilder;
		    this.fingerprintFactory = fingerprintFactory;
		}

		// ToDo - use concatenatable views here, not strings
		// ToDo - something better for errors
		public string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier)
		{
			return BuildRecentBuildsTable(projectSpecifier, null);
		}

		public string BuildRecentBuildsTable(IBuildSpecifier buildSpecifier)
		{
			return BuildRecentBuildsTable(buildSpecifier.ProjectSpecifier, buildSpecifier);
		}

		private string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier, IBuildSpecifier buildSpecifier)
		{
			Hashtable primaryContext = new Hashtable();
			Hashtable secondaryContext = new Hashtable();

			try
			{
				IBuildSpecifier[] mostRecentBuildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 10);
				secondaryContext["links"] = linkListFactory.CreateStyledBuildLinkList(mostRecentBuildSpecifiers, buildSpecifier, BuildReportBuildPlugin.ACTION_NAME);
				primaryContext["buildRows"] = velocityTransformer.Transform(@"BuildRows.vm", secondaryContext);
				primaryContext["allBuildsLink"] = linkFactory.CreateProjectLink(projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME);

				return velocityTransformer.Transform(@"RecentBuilds.vm", primaryContext);
			}
			catch (Exception)
			{
				// Assume exception also caught where we care about (i.e. by action)
				return "";
			}
		}

		public HtmlFragmentResponse GenerateAllBuildsView(IProjectSpecifier projectSpecifier)
		{
			Hashtable primaryContext = new Hashtable();
			Hashtable secondaryContext = new Hashtable();

			secondaryContext["links"] = linkListFactory.CreateStyledBuildLinkList(farmService.GetBuildSpecifiers(projectSpecifier), BuildReportBuildPlugin.ACTION_NAME);
			primaryContext["buildRows"] = velocityTransformer.Transform(@"BuildRows.vm", secondaryContext);
			primaryContext["allBuildsLink"] = linkFactory.CreateProjectLink(projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME);

			return velocityViewGenerator.GenerateView(@"AllBuilds.vm", primaryContext);
		}

	    public ConditionalGetFingerprint GetFingerprint(IRequest request)
	    {
	        ICruiseRequest cruiseRequest = new NameValueCruiseRequestFactory().CreateCruiseRequest(request, urlBuilder);
	        IBuildSpecifier mostRecentBuildSpecifier =
	            farmService.GetMostRecentBuildSpecifiers(cruiseRequest.ProjectSpecifier, 1)[0];
	        DateTime mostRecentBuildDate = new LogFile(mostRecentBuildSpecifier.BuildName).Date;
	        ConditionalGetFingerprint mostRecentBuildFingerprint =
	            fingerprintFactory.BuildFromDate(mostRecentBuildDate);
            ConditionalGetFingerprint mostRecentTemplateFingerprint = 
                fingerprintFactory.BuildFromFileNames(@"BuildRows.vm", @"RecentBuilds.vm", @"AllBuilds.vm");
	        return mostRecentBuildFingerprint.Combine(mostRecentTemplateFingerprint);
	    }
	}
}
