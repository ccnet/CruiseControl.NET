using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUserRequestSpecificSideBarViewBuilderTest
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock buildNameRetrieverMock;
		private DynamicMock recentBuildsViewBuilderMock;
		private DynamicMock pluginLinkCalculatorMock;
		private DefaultUserRequestSpecificSideBarViewBuilder viewBuilder;
		private DefaultServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			recentBuildsViewBuilderMock = new DynamicMock(typeof(IRecentBuildsViewBuilder));
			pluginLinkCalculatorMock = new DynamicMock(typeof(IPluginLinkCalculator));
			serverSpecifier = new DefaultServerSpecifier("myServer");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myProject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");

			viewBuilder = new DefaultUserRequestSpecificSideBarViewBuilder(new DefaultHtmlBuilder(), 
				(IUrlBuilder) urlBuilderMock.MockInstance, 
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance,
				(IRecentBuildsViewBuilder) recentBuildsViewBuilderMock.MockInstance,
				(IPluginLinkCalculator) pluginLinkCalculatorMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			buildNameRetrieverMock.Verify();
			recentBuildsViewBuilderMock.Verify();
			pluginLinkCalculatorMock.Verify();
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
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl1", new PropertyIs("ActionName", ViewServerLogAction.ACTION_NAME), serverSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", new PropertyIs("ActionName", DisplayAddProjectPageAction.ACTION_NAME), serverSpecifier);
			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "returnedurl1";
			expectedAnchor1.InnerHtml = "View Server Log";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "returnedurl2";
			expectedAnchor2.InnerHtml = "Add Project";

			// Execute
			HtmlTable table = viewBuilder.GetServerSideBar(serverSpecifier);

			Assert.IsTrue(TableContains(table, expectedAnchor1));
			Assert.IsTrue(TableContains(table, expectedAnchor2));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldReturnLinkToProjectReportForProjectView()
		{
			// Setup
			Mock link1Mock = new DynamicMock(typeof(IAbsoluteLink));
			Mock link2Mock = new DynamicMock(typeof(IAbsoluteLink));
			link1Mock.SetupResult("Description", "my link 1");
			link1Mock.SetupResult("AbsoluteURL", "myurl1");
			link2Mock.SetupResult("Description", "my link 2");
			link2Mock.SetupResult("AbsoluteURL", "myurl2");

			pluginLinkCalculatorMock.ExpectAndReturn("GetProjectPluginLinks", new IAbsoluteLink[] { (IAbsoluteLink) link1Mock.MockInstance, (IAbsoluteLink) link2Mock.MockInstance }, projectSpecifier);

/*
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "editUrl", new PropertyIs("ActionName", DisplayEditProjectPageAction.ACTION_NAME), projectSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "deleteUrl", new PropertyIs("ActionName", ShowDeleteProjectAction.ACTION_NAME), projectSpecifier);
*/
			HtmlTable buildsPanel = new HtmlTable();
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", buildsPanel, projectSpecifier);

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "myurl1";
			expectedAnchor1.InnerHtml = "my link 1";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "myurl2";
			expectedAnchor2.InnerHtml = "my link 2";

			// Execute
			HtmlTable table = viewBuilder.GetProjectSideBar(projectSpecifier);

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
			DefaultBuildSpecifier latestBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier,  "returnedLatestBuildName");
			DefaultBuildSpecifier nextBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier,  "returnedNextBuildName");
			DefaultBuildSpecifier previousBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier,  "returnedPreviousBuildName");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myCurrentBuild");

			buildNameRetrieverMock.ExpectAndReturn("GetLatestBuildSpecifier", latestBuildSpecifier, projectSpecifier);
			buildNameRetrieverMock.ExpectAndReturn("GetNextBuildSpecifier", nextBuildSpecifier, buildSpecifier);
			buildNameRetrieverMock.ExpectAndReturn("GetPreviousBuildSpecifier", previousBuildSpecifier, buildSpecifier);

			Mock link1Mock = new DynamicMock(typeof(IAbsoluteLink));
			Mock link2Mock = new DynamicMock(typeof(IAbsoluteLink));
			link1Mock.SetupResult("Description", "my link 1");
			link1Mock.SetupResult("AbsoluteURL", "myurl1");
			link2Mock.SetupResult("Description", "my link 2");
			link2Mock.SetupResult("AbsoluteURL", "myurl2");

			pluginLinkCalculatorMock.ExpectAndReturn("GetBuildPluginLinks", new IAbsoluteLink[] { (IAbsoluteLink) link1Mock.MockInstance, (IAbsoluteLink) link2Mock.MockInstance }, buildSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "latestUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), latestBuildSpecifier );
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "nextUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), nextBuildSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "previousUrl", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), previousBuildSpecifier );
			
			HtmlTable buildsPanel = new HtmlTable();
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", buildsPanel, projectSpecifier);

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
			expectedAnchor4.HRef = "myurl1";
			expectedAnchor4.InnerHtml = "my link 1";
			HtmlAnchor expectedAnchor5 = new HtmlAnchor();
			expectedAnchor5.HRef = "myurl2";
			expectedAnchor5.InnerHtml = "my link 2";

			// Execute
			HtmlTable table = viewBuilder.GetBuildSideBar(buildSpecifier);

			Assert.IsTrue(TableContains(table, expectedAnchor1));
			Assert.IsTrue(TableContains(table, expectedAnchor2));
			Assert.IsTrue(TableContains(table, expectedAnchor3));
			Assert.IsTrue(TableContains(table, expectedAnchor4));
			Assert.IsTrue(TableContains(table, expectedAnchor5));
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
