using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class SideBarViewBuilderTest
	{
		private DynamicMock userRequestSpecificSideBarViewBuilderMock;
		private SideBarViewBuilder sideBarViewBuilder;

		private DynamicMock cruiseRequestWrapperMock;

		[SetUp]
		public void Setup()
		{
			userRequestSpecificSideBarViewBuilderMock = new DynamicMock(typeof(IUserRequestSpecificSideBarViewBuilder));
			cruiseRequestWrapperMock = new DynamicMock(typeof(ICruiseRequest));

			sideBarViewBuilder = new SideBarViewBuilder(
				(IUserRequestSpecificSideBarViewBuilder) userRequestSpecificSideBarViewBuilderMock.MockInstance,
				(ICruiseRequest) cruiseRequestWrapperMock.MockInstance);
		}
		
		private void VerifyAll()
		{
			userRequestSpecificSideBarViewBuilderMock.Verify();
			cruiseRequestWrapperMock.Verify();
		}

		[Test]
		public void ShouldShowFarmViewIfNoServerSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetFarmSideBar", actionsControl);

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowServerViewIfServerButNoProjectSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetServerSideBar", actionsControl, serverSpecifier);

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowProjectViewIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetProjectSideBar", actionsControl, projectSpecifier);

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowBuildViewIfServerAndProjectAndBuildSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject"), "myBuild");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "myBuild");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetBuildSideBar", actionsControl, buildSpecifier);

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(actionsControl, sidebarControl);
			VerifyAll();
		}
	}
}
