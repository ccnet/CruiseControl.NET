using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// ToDo - actually look at html
	[TestFixture]
	public class TopControlsViewBuilderTest
	{
		private TopControlsViewBuilder viewBuilder;
		private DynamicMock urlBuilderMock;
		private DynamicMock buildNameFormatterMock;

		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildNameFormatterMock = new DynamicMock(typeof(IBuildNameFormatter));
			viewBuilder = new TopControlsViewBuilder(new DefaultHtmlBuilder(), (IUrlBuilder) urlBuilderMock.MockInstance, (IBuildNameFormatter) buildNameFormatterMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldShowJustLinkToDashboardIfNothingSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl", "default.aspx");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequest);
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldShowLinkToDashboardAndServerIfServerButNoProjectSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", "myServer");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequest);
			
			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldShowLinkToDashboardServerAndProjectIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", "myServer");
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl3", new PropertyIs("ActionName", CruiseActionFactory.VIEW_PROJECT_REPORT_ACTION_NAME), "myServer", "myProject");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequest);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldShowLinkToDashboardServerProjectAndBuildIfServerProjectAndBuildSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestMock.ExpectAndReturn("BuildName", "myBuild");
			buildNameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "pretty name", "myBuild");
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", "myServer");
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl3", new PropertyIs("ActionName", CruiseActionFactory.VIEW_PROJECT_REPORT_ACTION_NAME), "myServer", "myProject");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "returnedurl4", "BuildReport.aspx", "myServer", "myProject", "myBuild");

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute(cruiseRequest);

			// Verify
			VerifyAll();
		}
	}
}
