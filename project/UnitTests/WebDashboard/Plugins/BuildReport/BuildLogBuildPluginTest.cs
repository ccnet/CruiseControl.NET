using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.BuildReport
{
	[TestFixture]
	public class BuildLogBuildPluginTest
	{
		private BuildLogBuildPlugin buildPlugin;

		private DynamicMock requestMock;
		private DynamicMock buildRetrieverMock;
		private DynamicMock velocityViewGeneratorMock;

		private string buildLog;
		private Build build;
		private string buildLogLocation;
		private DefaultBuildSpecifier buildSpecifier;
		private IView view;

		[SetUp]
		public void Setup()
		{
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			requestMock = new DynamicMock(typeof(ICruiseRequest));

			buildPlugin = new BuildLogBuildPlugin((IBuildRetriever) buildRetrieverMock.MockInstance, (IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance);

			buildLog = "some stuff in a log with a < and >";
			buildLogLocation = "http://somewhere/mylog";
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"), "myproject"), "mybuild");
			build = new Build(buildSpecifier, buildLog, buildLogLocation);
			view = new HtmlView("foo");
		}

		private void VerifyAll()
		{
			requestMock.Verify();
			buildRetrieverMock.Verify();
			velocityViewGeneratorMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			requestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier);

			Hashtable expectedContext = new Hashtable();
			expectedContext["log"] = "some stuff in a log with a &lt; and &gt;";
			expectedContext["logUrl"] = buildLogLocation;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", view, "BuildLog.vm", new HashtableConstraint(expectedContext));

			// Execute & Verify
			Assert.AreEqual(view, buildPlugin.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}
	}
}
