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
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetServerSideBar", actionsControl, "myServer");

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
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetProjectSideBar", actionsControl, "myServer", "myProject");

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
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "myBuild");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetBuildSideBar", actionsControl, "myServer", "myProject", "myBuild");

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(actionsControl, sidebarControl);
			VerifyAll();
		}
	}
}
