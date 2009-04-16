using System;
using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
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
        private DynamicMock fingerprintFactoryMock;
        private DynamicMock urlBuilderMock;

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
		    fingerprintFactoryMock = new DynamicMock(typeof (IFingerprintFactory));
            urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));

			lister = new RecentBuildLister(
				(IFarmService) farmServiceMock.MockInstance,
				(IVelocityTransformer) velocityTransformerMock.MockInstance,
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance,
				(ILinkListFactory) linkListFactoryMock.MockInstance,
                (IFingerprintFactory) fingerprintFactoryMock.MockInstance,
                (ICruiseUrlBuilder)urlBuilderMock.MockInstance);

			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
            build2Specifier = new DefaultBuildSpecifier(projectSpecifier, "log20070401013456.xml");
			build1Specifier = new DefaultBuildSpecifier(projectSpecifier, "build1");
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			velocityTransformerMock.Verify();
			velocityViewGeneratorMock.Verify();
			linkFactoryMock.Verify();
			linkListFactoryMock.Verify();
            fingerprintFactoryMock.Verify();
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

	    [Test]
	    public void ShouldReturnFingerprintBasedOnLatestBuildDateAndVelocityTemplates()
	    {
            // TODO: Had to change content of build2specifier so that lister.GetFingerprint could new LogFile().Date
            // Would be nice to have a cleaner way of getting the date. Possibly from the specifier directly?
            const string testToken = "test token";

            DynamicMock requestMock = new DynamicMock(typeof(IRequest));
	        IRequest request = (IRequest) requestMock.MockInstance;

            DateTime olderDate = new DateTime(2007,1,1,1,1,1);
            DateTime mostRecentDate = new DateTime(2007, 4, 21, 1, 7, 8);

	        fingerprintFactoryMock.SetupResult("BuildFromDate", new ConditionalGetFingerprint(olderDate, testToken), typeof(DateTime));
            fingerprintFactoryMock.SetupResult("BuildFromFileNames", new ConditionalGetFingerprint(mostRecentDate, testToken), typeof(string[]));

            requestMock.SetupResult("SubFolders", new string[] {"server", "testServer", "project", "testProject", "build", "testBuild"});

            farmServiceMock.SetupResult("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { build2Specifier, build1Specifier }, typeof(IProjectSpecifier), typeof(int));

	        ConditionalGetFingerprint expectedFingerprint = new ConditionalGetFingerprint(mostRecentDate, testToken);

            Assert.AreEqual(expectedFingerprint, lister.GetFingerprint(request));
	    }
	}
}
