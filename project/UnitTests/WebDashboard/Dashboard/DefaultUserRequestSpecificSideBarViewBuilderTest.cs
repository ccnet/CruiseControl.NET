using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUserRequestSpecificSideBarViewBuilderTest
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
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", new PropertyIs("ActionName", DisplayAddProjectPageAction.ACTION_NAME));

			// Execute
			HtmlTable table = viewBuilder.GetFarmSideBar();
			HtmlAnchor anchor = new HtmlAnchor();
			anchor.HRef = "returnedurl";
			anchor.InnerHtml = "Add Project";

			Assert.IsTrue(TableContains(table, anchor));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnLinkToAddProjectAndServerLogForServerView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl1", new PropertyIs("ActionName", ViewServerLogAction.ACTION_NAME), "myServer");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", new PropertyIs("ActionName", DisplayAddProjectPageAction.ACTION_NAME), "myServer");
			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "returnedurl1";
			expectedAnchor1.InnerHtml = "View Server Log";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "returnedurl2";
			expectedAnchor2.InnerHtml = "Add Project";

			// Execute
			HtmlTable table = viewBuilder.GetServerSideBar("myServer");

			Assert.IsTrue(TableContains(table, expectedAnchor1));
			Assert.IsTrue(TableContains(table, expectedAnchor2));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnLinkToProjectReportForProjectView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "editUrl", new PropertyIs("ActionName", DisplayEditProjectPageAction.ACTION_NAME), "myServer", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "deleteUrl", new PropertyIs("ActionName", ShowDeleteProjectAction.ACTION_NAME), "myServer", "myProject");
			HtmlTable buildsPanel = new HtmlTable();
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", buildsPanel, "myServer", "myProject");

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "editUrl";
			expectedAnchor1.InnerHtml = "Edit Project";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "deleteUrl";
			expectedAnchor2.InnerHtml = "Delete Project";

			// Execute
			HtmlTable table = viewBuilder.GetProjectSideBar("myServer", "myProject");

			Assert.IsTrue(TableContains(table, expectedAnchor1));
			Assert.IsTrue(TableContains(table, expectedAnchor2));
			Assert.IsTrue(TableContains(table, buildsPanel));
			
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
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "latestUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), "myServer", "myProject", "returnedLatestBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "nextUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), "myServer", "myProject", "returnedNextBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "previousUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), "myServer", "myProject", "returnedPreviousBuildName");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "viewLogUrl", new PropertyIs("ActionName", ViewBuildLogAction.ACTION_NAME), "myServer", "myProject", "myCurrentBuild");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "viewTestDetailsUrl", new PropertyIs("ActionName", ViewTestDetailsBuildReportAction.ACTION_NAME), "myServer", "myProject", "myCurrentBuild");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "viewTestTimingsUrl", new PropertyIs("ActionName", ViewTestTimingsBuildReportAction.ACTION_NAME), "myServer", "myProject", "myCurrentBuild");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "viewFxCopUrl", new PropertyIs("ActionName", ViewFxCopBuildReportAction.ACTION_NAME), "myServer", "myProject", "myCurrentBuild");
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
			HtmlAnchor expectedAnchor5 = new HtmlAnchor();
			expectedAnchor5.HRef = "viewTestDetailsUrl";
			expectedAnchor5.InnerHtml = "View Test Details";
			HtmlAnchor expectedAnchor6 = new HtmlAnchor();
			expectedAnchor6.HRef = "viewTestTimingsUrl";
			expectedAnchor6.InnerHtml = "View Test Timings";
			HtmlAnchor expectedAnchor7 = new HtmlAnchor();
			expectedAnchor7.HRef = "viewFxCopUrl";
			expectedAnchor7.InnerHtml = "View FxCop Report";

			// Execute
			HtmlTable table = viewBuilder.GetBuildSideBar("myServer", "myProject", "myCurrentBuild");

			Assert.IsTrue(TableContains(table, expectedAnchor1));
			Assert.IsTrue(TableContains(table, expectedAnchor2));
			Assert.IsTrue(TableContains(table, expectedAnchor3));
			Assert.IsTrue(TableContains(table, expectedAnchor4));
			Assert.IsTrue(TableContains(table, expectedAnchor5));
			Assert.IsTrue(TableContains(table, expectedAnchor6));
			Assert.IsTrue(TableContains(table, expectedAnchor7));
			Assert.IsTrue(TableContains(table, buildsPanel));
			
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
