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
		private DefaultUserRequestSpecificSideBarViewBuilder viewBuilder;
		private DynamicMock urlBuilderMock;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			viewBuilder = new DefaultUserRequestSpecificSideBarViewBuilder(new DefaultHtmlBuilder(), (IUrlBuilder) urlBuilderMock.MockInstance);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
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
		public void ShouldReturnLinkToLatestProjectReportForProjectView()
		{
			// Setup
			urlBuilderMock.ExpectAndReturn("BuildProjectrUrl", "returnedurl1", "ProjectReport.aspx", "myServer", "myProject");

			HtmlAnchor expectedAnchor1 = new HtmlAnchor();
			expectedAnchor1.HRef = "returnedurl1";
			expectedAnchor1.InnerHtml = "Latest";

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.GetProjectSideBar("myServer", "myProject");

			Assert(TableContains(table, expectedAnchor1));
			
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
