using System.Web.UI;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// ToDo - actually look at html
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
			cruiseRequestWrapperMock.Verify();
		}

		[Test]
		public void ShouldShowJustLinkToDashboardIfNothingSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", "default.aspx");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequestWrapper);
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldShowLinkToDashboardAndServerIfServerButNoProjectSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl2", "default.aspx", "server=myServer");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequestWrapper);
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldShowLinkToDashboardServerAndProjectIfServerAndProjectSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("GetProjectName", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl2", "default.aspx", "server=myServer");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl3", "ProjectReport.aspx", "server=myServer&project=myProject");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequestWrapper);

			// Verify
			VerifyAll();
		}
	}
}
