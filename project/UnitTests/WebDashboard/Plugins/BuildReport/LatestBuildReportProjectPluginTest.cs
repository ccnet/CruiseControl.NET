using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.BuildReport
{
	[TestFixture]
	public class LatestBuildReportProjectPluginTest
	{
		private DynamicMock farmServiceMock;
		private DynamicMock linkFactoryMock;
		private LatestBuildReportProjectPlugin plugin;
		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			plugin = new LatestBuildReportProjectPlugin((IFarmService) farmServiceMock.MockInstance,
			                                            (ILinkFactory) linkFactoryMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}
		
		[Test]
		public void ShouldReturnWarningMessageIfNoBuildsAvailable()
		{
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, 1, null);
			
			IResponse returnedReponse = plugin.Execute(cruiseRequest);
			Assert.IsTrue(returnedReponse is HtmlFragmentResponse);
			Assert.AreEqual("There are no complete builds for this project", ((HtmlFragmentResponse) returnedReponse).ResponseFragment);
		}
		
		[Test]
		public void ShouldReturnRedirectToActualBuildReportPageIfBuildAvailable()
		{
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1, null);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", new GeneralAbsoluteLink("foo", "buildUrl"), buildSpecifier, BuildReportBuildPlugin.ACTION_NAME);

			IResponse returnedReponse = plugin.Execute(cruiseRequest);
			Assert.IsTrue(returnedReponse is RedirectResponse);
			Assert.AreEqual("buildUrl", ((RedirectResponse) returnedReponse).Url);
			
		}
	}
}
