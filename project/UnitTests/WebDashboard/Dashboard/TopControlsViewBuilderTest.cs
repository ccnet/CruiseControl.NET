using System.Web.UI.HtmlControls;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport;

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
		private DefaultServerSpecifier serverSpecifier;
		private DefaultProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildNameFormatterMock = new DynamicMock(typeof(IBuildNameFormatter));
			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			serverSpecifier = new DefaultServerSpecifier("myServer");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myProject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			viewBuilder = new TopControlsViewBuilder(new DefaultHtmlBuilder(), (IUrlBuilder) urlBuilderMock.MockInstance, 
				(IBuildNameFormatter) buildNameFormatterMock.MockInstance, (ICruiseRequest) cruiseRequestMock.MockInstance);

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
			HtmlTable table = (HtmlTable) viewBuilder.Execute();
			
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
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", serverSpecifier);

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute();
			
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
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", serverSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl3", new PropertyIs("ActionName", ViewProjectReportAction.ACTION_NAME), projectSpecifier);

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute();

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
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			cruiseRequestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			buildNameFormatterMock.ExpectAndReturn("GetPrettyBuildName", "pretty name", buildSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildUrl", "returnedurl1", "default.aspx");
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "returnedurl2", "default.aspx", serverSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "returnedurl3", new PropertyIs("ActionName", ViewProjectReportAction.ACTION_NAME), projectSpecifier);
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "returnedurl4", new PropertyIs("ActionName", ViewBuildReportAction.ACTION_NAME), buildSpecifier);

			// Execute
			HtmlTable table = (HtmlTable) viewBuilder.Execute();

			// Verify
			VerifyAll();
		}
	}
}
