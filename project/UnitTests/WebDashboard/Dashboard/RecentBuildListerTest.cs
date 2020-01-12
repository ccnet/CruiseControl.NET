using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
		private Mock<IFarmService> farmServiceMock;
		private Mock<IVelocityTransformer> velocityTransformerMock;
		private Mock<IVelocityViewGenerator> velocityViewGeneratorMock;
		private Mock<ILinkFactory> linkFactoryMock;
		private Mock<ILinkListFactory> linkListFactoryMock;
        private Mock<IFingerprintFactory> fingerprintFactoryMock;
        private Mock<ICruiseUrlBuilder> urlBuilderMock;

		private RecentBuildLister lister;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier build2Specifier;
		private DefaultBuildSpecifier build1Specifier;

	    [SetUp]
		public void Setup()
		{
			farmServiceMock = new Mock<IFarmService>();
			velocityTransformerMock = new Mock<IVelocityTransformer>();
			velocityViewGeneratorMock = new Mock<IVelocityViewGenerator>();
			linkFactoryMock = new Mock<ILinkFactory>();
			linkListFactoryMock = new Mock<ILinkListFactory>();
		    fingerprintFactoryMock = new Mock<IFingerprintFactory>();
            urlBuilderMock = new Mock<ICruiseUrlBuilder>();

			lister = new RecentBuildLister(
				(IFarmService) farmServiceMock.Object,
				(IVelocityTransformer) velocityTransformerMock.Object,
				(IVelocityViewGenerator) velocityViewGeneratorMock.Object,
				(ILinkFactory) linkFactoryMock.Object,
				(ILinkListFactory) linkListFactoryMock.Object,
                (IFingerprintFactory) fingerprintFactoryMock.Object,
                (ICruiseUrlBuilder)urlBuilderMock.Object,
                null);

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

			farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, 10, null)).Returns(buildSpecifiers).Verifiable();
			linkListFactoryMock.Setup(factory => factory.CreateStyledBuildLinkList(buildSpecifiers, build1Specifier, BuildReportBuildPlugin.ACTION_NAME)).Returns(buildLinks).Verifiable();
			velocityTransformerMock.Setup(transformer => transformer.Transform(@"BuildRows.vm", It.Is<Hashtable>(t => t.Count == 1 && t["links"] == buildLinks))).Returns(buildRows).Verifiable();

			IAbsoluteLink allBuildsLink = new GeneralAbsoluteLink("foo");
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME)).Returns(allBuildsLink).Verifiable();
			velocityTransformerMock.Setup(transformer => transformer.Transform(@"RecentBuilds.vm", It.Is<Hashtable>(t => t.Count == 2 && (string)t["buildRows"] == buildRows && t["allBuildsLink"] == allBuildsLink))).Returns(recentBuilds).Verifiable();

			Assert.AreEqual(recentBuilds, lister.BuildRecentBuildsTable(build1Specifier, null));

			VerifyAll();
		}

		[Test]
		public void ShouldBuildViewForAllBuilds()
		{
			IBuildSpecifier[] buildSpecifiers = new IBuildSpecifier [] {build2Specifier, build1Specifier };
			IAbsoluteLink[] buildLinks = new IAbsoluteLink[] { new GeneralAbsoluteLink("link1"), new GeneralAbsoluteLink("link2") };
			string buildRows = "renderred Links";
			HtmlFragmentResponse allBuildsResponse = new HtmlFragmentResponse("foo");

			farmServiceMock.Setup(service => service.GetBuildSpecifiers(projectSpecifier, null)).Returns(new IBuildSpecifier[] { build2Specifier, build1Specifier }).Verifiable();
			linkListFactoryMock.Setup(factory => factory.CreateStyledBuildLinkList(buildSpecifiers, BuildReportBuildPlugin.ACTION_NAME)).Returns(buildLinks).Verifiable();
			velocityTransformerMock.Setup(transformer => transformer.Transform(@"BuildRows.vm", It.Is<Hashtable>(t => t.Count == 1 && t["links"] == buildLinks))).Returns(buildRows).Verifiable();

			IAbsoluteLink allBuildsLink = new GeneralAbsoluteLink("foo");
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, "", ViewAllBuildsProjectPlugin.ACTION_NAME)).Returns(allBuildsLink).Verifiable();
			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"AllBuilds.vm", It.Is<Hashtable>(t => t.Count == 2 && (string)t["buildRows"] == buildRows && t["allBuildsLink"] == allBuildsLink))).Returns(allBuildsResponse).Verifiable();

			Assert.AreEqual(allBuildsResponse, lister.GenerateAllBuildsView(projectSpecifier, null));

			VerifyAll();
		}

	    [Test]
	    public void ShouldReturnFingerprintBasedOnLatestBuildDateAndVelocityTemplates()
	    {
            // TODO: Had to change content of build2specifier so that lister.GetFingerprint could new LogFile().Date
            // Would be nice to have a cleaner way of getting the date. Possibly from the specifier directly?
            const string testToken = "test token";

            var requestMock = new Mock<IRequest>();
	        IRequest request = (IRequest) requestMock.Object;

            DateTime olderDate = new DateTime(2007,1,1,1,1,1);
            DateTime mostRecentDate = new DateTime(2007, 4, 21, 1, 7, 8);

	        fingerprintFactoryMock.Setup(factory => factory.BuildFromDate(It.IsAny<DateTime>())).Returns(new ConditionalGetFingerprint(olderDate, testToken)).Verifiable();
            fingerprintFactoryMock.Setup(factory => factory.BuildFromFileNames(It.IsAny<string[]>())).Returns(new ConditionalGetFingerprint(mostRecentDate, testToken)).Verifiable();

            requestMock.SetupGet(_request => _request.SubFolders).Returns(new string[] {"server", "testServer", "project", "testProject", "build", "testBuild"}).Verifiable();

            farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(It.IsAny<IProjectSpecifier>(), It.IsAny<int>(), null)).Returns(new IBuildSpecifier[] { build2Specifier, build1Specifier }).Verifiable();

	        ConditionalGetFingerprint expectedFingerprint = new ConditionalGetFingerprint(mostRecentDate, testToken);

            Assert.AreEqual(expectedFingerprint, lister.GetFingerprint(request));
	    }
	}
}
