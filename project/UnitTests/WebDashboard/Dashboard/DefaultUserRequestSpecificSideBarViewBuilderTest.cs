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
