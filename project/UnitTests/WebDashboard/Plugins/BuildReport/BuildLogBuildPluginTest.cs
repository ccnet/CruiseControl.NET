using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.BuildReport
{
	[TestFixture]
	public class BuildLogBuildPluginTest
	{
		private BuildLogBuildPlugin buildPlugin;

		private DynamicMock requestMock;
		private DynamicMock buildRetrieverMock;
		private string serverName;
		private string projectName;
		private string buildName;
		private string buildLog;
		private Build build;
		private string buildLogLocation;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));
			requestMock = new DynamicMock(typeof(ICruiseRequest));

			buildPlugin = new BuildLogBuildPlugin((IBuildRetriever) buildRetrieverMock.MockInstance);

			serverName = "myserver";
			projectName = "myproject";
			buildName = "mybuild";
			buildLog = "some stuff in a log";
			buildLogLocation = "http://somewhere/mylog";
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName), buildName);
			build = new Build(buildSpecifier, buildLog, buildLogLocation);
		}

		private void VerifyAll()
		{
			requestMock.Verify();
			buildRetrieverMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			requestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier);

			// Execute
			IView view = buildPlugin.Execute((ICruiseRequest) requestMock.MockInstance);
			Assert.IsTrue(((HtmlGenericControl) view.Control).InnerHtml.IndexOf(buildLog) > 0);
			Assert.IsTrue(((HtmlGenericControl) view.Control).InnerHtml.IndexOf(buildLogLocation) > 0);

			VerifyAll();
		}
	}
}
