using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class RecentBuildListerTest : Assertion
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock farmMock;
		private DynamicMock nameFormatterMock;
		private RecentBuildLister Builder;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			farmMock = new DynamicMock(typeof(IFarmService));
			nameFormatterMock = new DynamicMock(typeof(IBuildNameFormatter));
			Builder = new RecentBuildLister(new DefaultHtmlBuilder(), 
				(IUrlBuilder) urlBuilderMock.MockInstance,
				(IFarmService) farmMock.MockInstance,
				(IBuildNameFormatter) nameFormatterMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			farmMock.Verify();
			nameFormatterMock.Verify();
		}

		[Test]
		public void ShouldRequestRecentBuildsFromServerAndDisplayARowForEachOne()
		{
			// Setup
			farmMock.ExpectAndReturn("GetMostRecentBuildNames", new string [] {"build2", "build1"}, "myServer", "myProject", 10);
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "url1", "BuildReport.aspx", "myServer", "myProject", "build2");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "url2", "BuildReport.aspx", "myServer", "myProject", "build1");
			nameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "prettyName2", "build2");
			nameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "prettyName1", "build1");

			// Execute
			HtmlTable builtTable = Builder.BuildRecentBuildsPanel("myServer", "myProject");

			// Verify
			AssertEquals(2, builtTable.Rows.Count);

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "url1";
			expectedAnchor1.InnerHtml = "prettyName2";
			HtmlAnchor expectedAnchor2 = new HtmlAnchor();
			expectedAnchor2.HRef = "url2";
			expectedAnchor2.InnerHtml = "prettyName1";

			Assert(expectedAnchor1.HRef, TableContains(builtTable, expectedAnchor1));
			Assert(expectedAnchor2.HRef, TableContains(builtTable, expectedAnchor2));

			VerifyAll();
		}

		[Test]
		public void ShouldRequestRecentBuildsFromServerAndShowNothingIfNoBuilds()
		{
			// Setup
			farmMock.ExpectAndReturn("GetMostRecentBuildNames", new string [0], "myServer", "myProject", 10);
			urlBuilderMock.ExpectNoCall("BuildBuildUrl",typeof(string),typeof(string),typeof(string),typeof(string));

			// Execute
			HtmlTable builtTable = (HtmlTable) Builder.BuildRecentBuildsPanel("myServer", "myProject");

			// Verify
			AssertEquals(0, builtTable.Rows.Count);

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
