using System;
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
	public class DecoratingRecentBuildsPanelBuilderTest : Assertion
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock decoratedBuilderMock;
		private DecoratingRecentBuildsPanelBuilder builder;
		private IHtmlBuilder htmlBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			decoratedBuilderMock = new DynamicMock(typeof(IRecentBuildsPanelViewBuilder));

			htmlBuilder = new DefaultHtmlBuilder();
			builder = new DecoratingRecentBuildsPanelBuilder(htmlBuilder, 
				(IUrlBuilder) urlBuilderMock.MockInstance,
				(IRecentBuildsPanelViewBuilder) decoratedBuilderMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			decoratedBuilderMock.Verify();
		}

		[Test]
		public void ShouldPutAHeaderRowAtTop()
		{
			// Setup
			HtmlTable table = htmlBuilder.CreateTable(htmlBuilder.CreateRow(htmlBuilder.CreateCell("hello decorator")));
			decoratedBuilderMock.ExpectAndReturn("BuildRecentBuildsPanel", table, "myServer", "myProject");

			// Execute
			HtmlTable returnedTable = builder.BuildRecentBuildsPanel("myServer", "myProject");

			// Verify
			AssertEquals("Recent Builds", returnedTable.Rows[0].Cells[0].InnerHtml);
			AssertEquals("hello decorator", returnedTable.Rows[1].Cells[0].InnerHtml);
			VerifyAll();
		}

		[Test]
		public void ShouldPutAFooterRowAtBottom()
		{
			// Setup
			HtmlTable table = htmlBuilder.CreateTable(htmlBuilder.CreateRow(htmlBuilder.CreateCell("hello decorator")));
			decoratedBuilderMock.ExpectAndReturn("BuildRecentBuildsPanel", table, "myServer", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl1", "Controller.aspx", new PropertyIs("ActionName", CruiseActionFactory.VIEW_ALL_BUILDS_ACTION_NAME), "myServer", "myProject");

			// Execute
			HtmlTable returnedTable = builder.BuildRecentBuildsPanel("myServer", "myProject");

			// Verify
			AssertEquals("hello decorator", returnedTable.Rows[1].Cells[0].InnerHtml);
			HtmlAnchor showAllLink = (HtmlAnchor) returnedTable.Rows[2].Cells[0].Controls[0];
			AssertEquals("Show All", showAllLink.InnerHtml);
			AssertEquals("returnedurl1", showAllLink.HRef);
			VerifyAll();
		}
	}
}
