using System;
using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DecoratingRecentBuildsPanelBuilderTest
	{
		private DynamicMock urlBuilderMock;
		private DynamicMock decoratedBuilderMock;
		private DecoratingRecentBuildsPanelBuilder builder;
		private IHtmlBuilder htmlBuilder;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			decoratedBuilderMock = new DynamicMock(typeof(IRecentBuildsViewBuilder));

			htmlBuilder = new DefaultHtmlBuilder();
			builder = new DecoratingRecentBuildsPanelBuilder(htmlBuilder, 
				(IUrlBuilder) urlBuilderMock.MockInstance,
				(IRecentBuildsViewBuilder) decoratedBuilderMock.MockInstance);
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
			decoratedBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", table, "myServer", "myProject");

			// Execute
			HtmlTable returnedTable = builder.BuildRecentBuildsTable("myServer", "myProject");

			// Verify
			Assert.AreEqual("Recent Builds", returnedTable.Rows[0].Cells[0].InnerHtml);
			Assert.AreEqual("hello decorator", returnedTable.Rows[1].Cells[0].InnerHtml);
			VerifyAll();
		}

		[Test]
		public void ShouldPutAFooterRowAtBottom()
		{
			// Setup
			HtmlTable table = htmlBuilder.CreateTable(htmlBuilder.CreateRow(htmlBuilder.CreateCell("hello decorator")));
			decoratedBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", table, "myServer", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl1", new PropertyIs("ActionName", ViewAllBuildsAction.ACTION_NAME), "myServer", "myProject");

			// Execute
			HtmlTable returnedTable = builder.BuildRecentBuildsTable("myServer", "myProject");

			// Verify
			// Row 0 is header
			// Row 1 is row returned from decoratedBuilder
			Assert.AreEqual("hello decorator", returnedTable.Rows[1].Cells[0].InnerHtml);
			// Row 2 is a blank row
			// Row 3 is footer
			HtmlAnchor showAllLink = (HtmlAnchor) returnedTable.Rows[3].Cells[0].Controls[0];
			Assert.AreEqual("Show All", showAllLink.InnerHtml);
			Assert.AreEqual("returnedurl1", showAllLink.HRef);
			VerifyAll();
		}
	}
}
