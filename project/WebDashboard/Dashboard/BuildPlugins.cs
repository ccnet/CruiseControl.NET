using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildPlugins : IBuildPluginLinkCalculator
	{
		private readonly IBuildLinkFactory buildLinkFactory;

		public BuildPlugins(IBuildLinkFactory buildLinkFactory)
		{
			this.buildLinkFactory = buildLinkFactory;
		}

		public IAbsoluteLink[] GetBuildPluginLinks(string serverName, string projectName, string buildName)
		{
			ArrayList links = new ArrayList();

			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View Build Log", new ActionSpecifierWithName(ViewBuildLogAction.ACTION_NAME)));
			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View Test Details", new ActionSpecifierWithName(ViewTestDetailsBuildReportAction.ACTION_NAME)));
			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View Test Timings", new ActionSpecifierWithName(ViewTestTimingsBuildReportAction.ACTION_NAME)));
			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View NAnt Report", new ActionSpecifierWithName(ViewNAntBuildReportAction.ACTION_NAME)));
			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View FxCop Report", new ActionSpecifierWithName(ViewFxCopBuildReportAction.ACTION_NAME)));
			links.Add(buildLinkFactory.CreateBuildLink(serverName, projectName, buildName, "View NCover Report", new ActionSpecifierWithName(ViewNCoverBuildReportAction.ACTION_NAME)));

			return (IAbsoluteLink[]) links.ToArray(typeof (IAbsoluteLink));
		}
	}
}
