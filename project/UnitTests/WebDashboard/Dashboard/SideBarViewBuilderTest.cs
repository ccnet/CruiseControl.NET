using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class SideBarViewBuilderTest : Assertion
	{
		private DynamicMock userRequestSpecificSideBarViewBuilderMock;
		private SideBarViewBuilder sideBarViewBuilder;

		private DynamicMock cruiseRequestWrapperMock;
		private ICruiseRequestWrapper cruiseRequestWrapper;

		[SetUp]
		public void Setup()
		{
			userRequestSpecificSideBarViewBuilderMock = new DynamicMock(typeof(IUserRequestSpecificSideBarViewBuilder));
			sideBarViewBuilder = new SideBarViewBuilder((IUserRequestSpecificSideBarViewBuilder) userRequestSpecificSideBarViewBuilderMock.MockInstance);

			cruiseRequestWrapperMock = new DynamicMock(typeof(ICruiseRequestWrapper));
			cruiseRequestWrapper = (ICruiseRequestWrapper) cruiseRequestWrapperMock.MockInstance;
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
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetFarmSideBar", actionsControl);

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute(cruiseRequestWrapper);

			// Verify
			AssertEquals(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowServerViewIfServerButNoProjectSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetServerSideBar", actionsControl, "myServer");

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute(cruiseRequestWrapper);

			// Verify
			AssertEquals(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowProjectViewIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("GetBuildName", "");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetProjectSideBar", actionsControl, "myServer", "myProject");

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute(cruiseRequestWrapper);

			// Verify
			AssertEquals(actionsControl, sidebarControl);
			VerifyAll();
		}

		[Test]
		public void ShouldShowBuildViewIfServerAndProjectAndBuildSpecified()
		{
			// Setup
			HtmlTable actionsControl = new HtmlTable();
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("GetBuildName", "myBuild");
			userRequestSpecificSideBarViewBuilderMock.ExpectAndReturn("GetBuildSideBar", actionsControl, "myServer", "myProject", "myBuild");

			// Execute
			HtmlTable sidebarControl = sideBarViewBuilder.Execute(cruiseRequestWrapper);

			// Verify
			AssertEquals(actionsControl, sidebarControl);
			VerifyAll();
		}
	}
}
