using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUserRequestSpecificSideBarViewBuilderTest : Assertion
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock buildNameRetrieverMock;
		private DefaultUserRequestSpecificSideBarViewBuilder viewBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			viewBuilder = new DefaultUserRequestSpecificSideBarViewBuilder(new DefaultHtmlBuilder(), 
				(IUrlBuilder) urlBuilderMock.MockInstance, 
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			buildNameRetrieverMock.Verify();
		}

		[Test]
		public void ShouldReturnLinkToAddProjectForFarmView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", "controller.aspx");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.GetFarmSideBar();
			HtmlAnchor anchor = new HtmlAnchor();
			anchor.HRef = "returnedurl";
			anchor.InnerHtml = "Add Project";

			Assert(TableContains(table, anchor));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnLinkToAddProjectAndServerLogForServerView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl1", "ViewServerLog.aspx", "myServer");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "controller.aspx", "myServer");
			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "returnedurl1";
			expectedAnchor1.InnerHtml = "View Server Log";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "returnedurl2";
			expectedAnchor2.InnerHtml = "Add Project";

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.GetServerSideBar("myServer");

			Assert(TableContains(table, expectedAnchor1));
			Assert(TableContains(table, expectedAnchor2));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnLinkToLatestBuildReportForProjectView()
		{
			// Setup
			buildNameRetrieverMock.ExpectAndReturn("GetLatestBuildName", "returnedLatestBuildName", "myServer", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "latestUrl", "BuildReport.aspx", "myServer", "myProject", "returnedLatestBuildName");

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "latestUrl";
			expectedAnchor1.InnerHtml = "Latest";

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.GetProjectSideBar("myServer", "myProject");

			Assert(TableContains(table, expectedAnchor1));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCorrectLinksForBuildView()
		{
			// Setup
			buildNameRetrieverMock.ExpectAndReturn("GetLatestBuildName", "returnedLatestBuildName", "myServer", "myProject");
			buildNameRetrieverMock.ExpectAndReturn("GetNextBuildName", "returnedNextBuildName", "myServer", "myProject", "myCurrentBuild");
			buildNameRetrieverMock.ExpectAndReturn("GetPreviousBuildName", "returnedPreviousBuildName", "myServer", "myProject", "myCurrentBuild");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "latestUrl", "BuildReport.aspx", "myServer", "myProject", "returnedLatestBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "nextUrl", "BuildReport.aspx", "myServer", "myProject", "returnedNextBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "previousUrl", "BuildReport.aspx", "myServer", "myProject", "returnedPreviousBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "viewLogUrl", "ViewLog.aspx", "myServer", "myProject", "myCurrentBuild");

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "latestUrl";
			expectedAnchor1.InnerHtml = "Latest";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "nextUrl";
			expectedAnchor2.InnerHtml = "Next";
			HtmlAnchor expectedAnchor3 = new HtmlAnchor();
			expectedAnchor3.HRef = "previousUrl";
			expectedAnchor3.InnerHtml = "Previous";
			HtmlAnchor expectedAnchor4 = new HtmlAnchor();
			expectedAnchor4.HRef = "viewLogUrl";
			expectedAnchor4.InnerHtml = "View Build Log";

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.GetBuildSideBar("myServer", "myProject", "myCurrentBuild");

			Assert(TableContains(table, expectedAnchor1));
			Assert(TableContains(table, expectedAnchor2));
			Assert(TableContains(table, expectedAnchor3));
			Assert(TableContains(table, expectedAnchor4));
			
			// Verify
			VerifyAll();
		}

		private bool TableContains(HtmlTable table, HtmlAnchor anchor)
		{
			foreach (HtmlTableRow row in table.Rows)
			{
				foreach (HtmlTableCell cell in row.Cells)
				{
					foreach (Control control in cell.Controls)
					{
						if (control is HtmlAnchor)
						{
							HtmlAnchor currentAnchor = (HtmlAnchor) control;
							if (currentAnchor.HRef == anchor.HRef && currentAnchor.InnerHtml == anchor.InnerHtml)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}
