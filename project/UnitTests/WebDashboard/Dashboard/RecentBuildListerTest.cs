using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class RecentBuildListerTest
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock farmServiceMock;
		private DynamicMock nameFormatterMock;
		private RecentBuildLister Builder;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier build2Specifier;
		private DefaultBuildSpecifier build1Specifier;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			nameFormatterMock = new DynamicMock(typeof(IBuildNameFormatter));
			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			build2Specifier = new DefaultBuildSpecifier(projectSpecifier, "build2");
			build1Specifier = new DefaultBuildSpecifier(projectSpecifier, "build1");

			Builder = new RecentBuildLister(new DefaultHtmlBuilder(), 
				(IUrlBuilder) urlBuilderMock.MockInstance,
				(IFarmService) farmServiceMock.MockInstance,
				(IBuildNameFormatter) nameFormatterMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			farmServiceMock.Verify();
			nameFormatterMock.Verify();
		}

		[Test]
		public void ShouldRequestRecentBuildsFromServerAndDisplayARowForEachOne()
		{
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier [] {build2Specifier, build1Specifier }, projectSpecifier, 10);
			SetupBuildExpectations();
			HtmlTable builtTable = Builder.BuildRecentBuildsTable(projectSpecifier);
			CheckReturnedTableForCorrectBuilds(builtTable);
		}

		[Test]
		public void ShouldRequestAllBuildsFromServerAndDisplayARowForEachOne()
		{
			farmServiceMock.ExpectAndReturn("GetBuildSpecifiers", new IBuildSpecifier [] { build2Specifier, build1Specifier }, projectSpecifier);
			SetupBuildExpectations();
			HtmlTable builtTable = Builder.BuildAllBuildsTable(projectSpecifier);
			CheckReturnedTableForCorrectBuilds(builtTable);
		}

		private void SetupBuildExpectations()
		{
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "url1", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), build2Specifier);
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "url2", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), build1Specifier);
			nameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "prettyName2", build2Specifier);
			nameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "prettyName1", build1Specifier);
			nameFormatterMock.ExpectAndReturn("GetCssClassForBuildLink", "css2", build2Specifier);
			nameFormatterMock.ExpectAndReturn("GetCssClassForBuildLink", "css1", build1Specifier);
		}

		private void CheckReturnedTableForCorrectBuilds(HtmlTable builtTable)
		{

			// Verify
			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "url1";
			expectedAnchor1.InnerHtml = "prettyName2";
			expectedAnchor1.Attributes["class"] = "css1";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "url2";
			expectedAnchor2.InnerHtml = "prettyName1";
			expectedAnchor1.Attributes["class"] = "css2";

			Assert.IsTrue(TableContains(builtTable, expectedAnchor1));
			Assert.IsTrue(TableContains(builtTable, expectedAnchor2));

			VerifyAll();
		}

		[Test]
		public void ShouldRequestRecentBuildsFromServerAndShowNothingIfNoBuilds()
		{
			// Setup
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier [0], projectSpecifier, 10);
			urlBuilderMock.ExpectNoCall("BuildBuildUrl",typeof(IActionSpecifier), typeof(IBuildSpecifier));

			// Execute
			HtmlTable builtTable = Builder.BuildRecentBuildsTable(projectSpecifier);

			// Verify
			Assert.AreEqual(0, builtTable.Rows.Count);

			VerifyAll();
		}

		[Test]
		public void ShouldRequestAllBuildsFromServerAndShowNothingIfNoBuilds()
		{
			// Setup
			farmServiceMock.ExpectAndReturn("GetBuildSpecifiers", new IBuildSpecifier [0], projectSpecifier);
			urlBuilderMock.ExpectNoCall("BuildBuildUrl", typeof(IActionSpecifier), typeof(IBuildSpecifier));

			// Execute
			HtmlTable builtTable = Builder.BuildAllBuildsTable(projectSpecifier);

			// Verify
			// Just comment rows
			Assert.AreEqual(2, builtTable.Rows.Count);

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
