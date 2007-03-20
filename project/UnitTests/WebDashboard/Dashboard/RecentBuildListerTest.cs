using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class RecentBuildListerTest
	{
		private DynamicMock farmServiceMock;
		private DynamicMock velocityTransformerMock;
		private DynamicMock velocityViewGeneratorMock;
		private DynamicMock linkFactoryMock;
		private DynamicMock linkListFactoryMock;

		private RecentBuildLister lister;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier build2Specifier;
		private DefaultBuildSpecifier build1Specifier;

		[SetUp]
		public void Setup()
		{
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			velocityTransformerMock = new DynamicMock(typeof(IVelocityTransformer));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			linkListFactoryMock = new DynamicMock(typeof(ILinkListFactory));

			lister = new RecentBuildLister(
				(IFarmService) farmServiceMock.MockInstance,
				(IVelocityTransformer) velocityTransformerMock.MockInstance,
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance,
				(ILinkListFactory) linkListFactoryMock.MockInstance);

			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			build2Specifier = new DefaultBuildSpecifier(projectSpecifier, "build2");
			build1Specifier = new DefaultBuildSpecifier(projectSpecifier, "build1");
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			velocityTransformerMock.Verify();
			velocityViewGeneratorMock.Verify();
			linkFactoryMock.Verify();
			linkListFactoryMock.Verify();
		}

		[Test]
		public void ShouldBuildViewForRecentBuilds()
		{
			IBuildSpecifier[] buildSpecifiers = new IBuildSpecifier [] {build2Specifier, build1Specifier };
			IAbsoluteLink[] buildLinks = new IAbsoluteLink[] { new GeneralAbsoluteLink("link1"), new GeneralAbsoluteLink("link2") };
			string buildRows = "renderred Links";
			string recentBuilds = "recentBuilds";
			Hashtable context1 = new Hashtable();
			Hashtable context2 = new Hashtable();

			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", buildSpecifiers, projectSpecifier, 10);
			linkListFactoryMock.ExpectAndReturn("CreateStyledBuildLinkList", buildLinks, buildSpecifiers, build1Specifier, BuildReportBuildPlugin.ACTION_NAME);
			context1["links"] = buildLinks;
			velocityTransformerMock.ExpectAndReturn("Transform", buildRows, @"BuildRows.vm", new HashtableConstraint(context1));

			context2["buildRows"] = buildRows;
			IAbsoluteLink allBuildsLink = new GeneralAbsoluteLink("foo");
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", allBuildsLink, projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME);
			context2["allBuildsLink"] = allBuildsLink;
			velocityTransformerMock.ExpectAndReturn("Transform", recentBuilds, @"RecentBuilds.vm", new HashtableConstraint(context2));

			Assert.AreEqual(recentBuilds, lister.BuildRecentBuildsTable(build1Specifier));

			VerifyAll();
		}

		[Test]
		public void ShouldBuildViewForAllBuilds()
		{
			IBuildSpecifier[] buildSpecifiers = new IBuildSpecifier [] {build2Specifier, build1Specifier };
			IAbsoluteLink[] buildLinks = new IAbsoluteLink[] { new GeneralAbsoluteLink("link1"), new GeneralAbsoluteLink("link2") };
			string buildRows = "renderred Links";
			IResponse allBuildsResponse = new HtmlFragmentResponse("foo");
			Hashtable context1 = new Hashtable();
			Hashtable context2 = new Hashtable();

			farmServiceMock.ExpectAndReturn("GetBuildSpecifiers", new IBuildSpecifier [] { build2Specifier, build1Specifier }, projectSpecifier);
			linkListFactoryMock.ExpectAndReturn("CreateStyledBuildLinkList", buildLinks, buildSpecifiers, BuildReportBuildPlugin.ACTION_NAME);
			context1["links"] = buildLinks;
			velocityTransformerMock.ExpectAndReturn("Transform", buildRows, @"BuildRows.vm", new HashtableConstraint(context1));

			context2["buildRows"] = buildRows;
			IAbsoluteLink allBuildsLink = new GeneralAbsoluteLink("foo");
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", allBuildsLink, projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME);
			context2["allBuildsLink"] = allBuildsLink;
			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", allBuildsResponse, @"AllBuilds.vm", new HashtableConstraint(context2));

			Assert.AreEqual(allBuildsResponse, lister.GenerateAllBuildsView(projectSpecifier));

			VerifyAll();
		}
	}
}
