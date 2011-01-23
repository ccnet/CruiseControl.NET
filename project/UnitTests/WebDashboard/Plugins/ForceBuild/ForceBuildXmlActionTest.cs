using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ForceBuild
{
	[TestFixture]
	public class ForceBuildXmlActionTest
	{
		private DynamicMock mockFarmService;
		private ForceBuildXmlAction reportAction;
		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void SetUp()
		{
			mockFarmService = new DynamicMock(typeof (IFarmService));
			reportAction = new ForceBuildXmlAction((IFarmService) mockFarmService.MockInstance);
			cruiseRequestMock = new DynamicMock(typeof (ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		public void VerifyAll()
		{
			mockFarmService.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldReturnCorrectMessageIfBuildForcedSuccessfully()
		{
			DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(
				new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.SetupResult("ProjectSpecifier", projectSpecifier);
			cruiseRequestMock.SetupResult("ProjectName", "myProject");

            mockFarmService.Expect("ForceBuild", projectSpecifier, (string)null);

			IResponse response = reportAction.Execute(cruiseRequest);
			Assert.IsTrue(response is XmlFragmentResponse);
			Assert.AreEqual("<ForceBuildResult>Build Forced for myProject</ForceBuildResult>",
			                ((XmlFragmentResponse) response).ResponseFragment);
		}
	}
}