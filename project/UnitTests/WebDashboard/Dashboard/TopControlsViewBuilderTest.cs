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
	public class TopControlsViewBuilderTest : Assertion
	{
		private TopControlsViewBuilder viewBuilder;
		private DynamicMock urlBuilderMock;

		private DynamicMock cruiseRequestWrapperMock;
		private ICruiseRequestWrapper cruiseRequestWrapper;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			viewBuilder = new TopControlsViewBuilder(new DefaultHtmlBuilder(), (IUrlBuilder) urlBuilderMock.MockInstance);

			cruiseRequestWrapperMock = new DynamicMock(typeof(ICruiseRequestWrapper));
			cruiseRequestWrapper = (ICruiseRequestWrapper) cruiseRequestWrapperMock.MockInstance;
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldShowJustLinkToDashboardIfNothingSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", "default.aspx");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequestWrapper);
			HtmlAnchor anchor = new HtmlAnchor();
			anchor.HRef = "returnedurl";
			anchor.InnerHtml = "Dashboard";

			Assert(TableContains(table, anchor));
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldAlsoShowLinkToServerIfServerSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl2", "default.aspx", "server=myServer");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequestWrapper);

			HtmlAnchor anchor1 = new HtmlAnchor();
			anchor1.HRef = "returnedurl1";
			anchor1.InnerHtml = "Dashboard";

			HtmlAnchor anchor2 = new HtmlAnchor();
			anchor2.HRef = "returnedurl2";
			anchor2.InnerHtml = "myServer";

			// To Do - easier way to test this? Look at html, maybe?
//			Assert(TableContains(table, anchor1));
//			Assert(TableContains(table, anchor2));
			
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
