using System;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
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

			// Execute
			HtmlTable returnedTable = builder.BuildRecentBuildsPanel("myServer", "myProject");

			// Verify
			AssertEquals("hello decorator", returnedTable.Rows[1].Cells[0].InnerHtml);
			AssertEquals("Show All", returnedTable.Rows[2].Cells[0].InnerHtml);
			VerifyAll();
		}
	}
}
