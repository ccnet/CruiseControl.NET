using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildPluginsTest
	{
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private BuildPlugins buildPlugins;
		private DynamicMock buildLinkFactoryMock;

		[SetUp]
		public void Setup()
		{
			buildLinkFactoryMock = new DynamicMock(typeof(IBuildLinkFactory));
			buildPlugins = new BuildPlugins((IBuildLinkFactory) buildLinkFactoryMock.MockInstance);
		}

		private void VerifyAll()
		{
			buildLinkFactoryMock.Verify();
		}

		[Test]
		public void ShouldReturnBuildLinks()
		{
			IAbsoluteLink viewBuildLogLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink viewTestDetailsLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink viewTestTimingsLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink viewNAntLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink viewFxCopLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink viewNCoverLink = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewBuildLogLink, serverName, projectName, buildName, "View Build Log", new PropertyIs("ActionName", ViewBuildLogAction.ACTION_NAME));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewTestDetailsLink, serverName, projectName, buildName, "View Test Details", new PropertyIs("ActionName", ViewTestDetailsBuildReportAction.ACTION_NAME));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewTestTimingsLink, serverName, projectName, buildName, "View Test Timings", new PropertyIs("ActionName", ViewTestTimingsBuildReportAction.ACTION_NAME));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewNAntLink, serverName, projectName, buildName, "View NAnt Report", new PropertyIs("ActionName", ViewNAntBuildReportAction.ACTION_NAME));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewFxCopLink, serverName, projectName, buildName, "View FxCop Report", new PropertyIs("ActionName", ViewFxCopBuildReportAction.ACTION_NAME));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", viewNCoverLink, serverName, projectName, buildName, "View NCover Report", new PropertyIs("ActionName", ViewNCoverBuildReportAction.ACTION_NAME));

			IAbsoluteLink[] buildLinks = buildPlugins.GetBuildPluginLinks(serverName, projectName, buildName);

			Assert.AreSame(viewBuildLogLink, buildLinks[0]);
			Assert.AreSame(viewTestDetailsLink, buildLinks[1]);
			Assert.AreSame(viewTestTimingsLink, buildLinks[2]);
			Assert.AreSame(viewNAntLink, buildLinks[3]);
			Assert.AreSame(viewFxCopLink, buildLinks[4]);
			Assert.AreSame(viewNCoverLink, buildLinks[5]);

			VerifyAll();
		}
	}
}
