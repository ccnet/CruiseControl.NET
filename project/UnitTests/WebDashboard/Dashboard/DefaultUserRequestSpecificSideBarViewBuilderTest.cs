using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUserRequestSpecificSideBarViewBuilderTest : Assertion
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock buildNameRetrieverMock;
		private DynamicMock recentBuildsViewBuilderMock;
		private DefaultUserRequestSpecificSideBarViewBuilder viewBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			recentBuildsViewBuilderMock = new DynamicMock(typeof(IRecentBuildsViewBuilder));
			viewBuilder = new DefaultUserRequestSpecificSideBarViewBuilder(new DefaultHtmlBuilder(), 
				(IUrlBuilder) urlBuilderMock.MockInstance, 
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance,
				(IRecentBuildsViewBuilder) recentBuildsViewBuilderMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			buildNameRetrieverMock.Verify();
			recentBuildsViewBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnLinkToAddProjectForFarmView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", new PropertyIs("ActionName", CruiseActionFactory.ADD_PROJECT_DISPLAY_ACTION_NAME));

			// Execute
			HtmlTable table = viewBuilder.GetFarmSideBar();
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
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", new PropertyIs("ActionName", CruiseActionFactory.ADD_PROJECT_DISPLAY_ACTION_NAME), "myServer");
			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "returnedurl1";
			expectedAnchor1.InnerHtml = "View Server Log";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "returnedurl2";
			expectedAnchor2.InnerHtml = "Add Project";

			// Execute
			HtmlTable table = viewBuilder.GetServerSideBar("myServer");

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
			HtmlTable table = viewBuilder.GetProjectSideBar("myServer", "myProject");

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
			HtmlTable buildsPanel = new HtmlTable();
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", buildsPanel, "myServer", "myProject");

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
			HtmlTable table = viewBuilder.GetBuildSideBar("myServer", "myProject", "myCurrentBuild");

			Assert(TableContains(table, expectedAnchor1));
			Assert(TableContains(table, expectedAnchor2));
			Assert(TableContains(table, expectedAnchor3));
			Assert(TableContains(table, expectedAnchor4));
			Assert(TableContains(table, buildsPanel));
			
			// Verify
			VerifyAll();
		}

		private bool TableContains(HtmlTable table, Control expectedControl)
		{
			foreach (HtmlTableRow row in table.Rows)
			{
				foreach (HtmlTableCell cell in row.Cells)
				{
					foreach (Control control in cell.Controls)
					{
						if (control is HtmlAnchor && expectedControl is HtmlAnchor)
						{
							HtmlAnchor currentAnchor = (HtmlAnchor) control;
							HtmlAnchor expectedAnchor = (HtmlAnchor) expectedControl;
							if (currentAnchor.HRef == expectedAnchor.HRef && currentAnchor.InnerHtml == expectedAnchor.InnerHtml)
							{
								return true;
							}
						}
						else if (control == expectedControl)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
