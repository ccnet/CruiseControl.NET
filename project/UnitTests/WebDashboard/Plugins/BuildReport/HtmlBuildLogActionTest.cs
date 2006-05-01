using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.BuildReport
{
	[TestFixture]
	public class HtmlBuildLogActionTest
	{
		private HtmlBuildLogAction buildLogAction;

		private DynamicMock requestMock;
		private DynamicMock buildRetrieverMock;
		private DynamicMock urlBuilderMock;
		private DynamicMock velocityViewGeneratorMock;

		private string buildLog;
		private Build build;
		private DefaultBuildSpecifier buildSpecifier;
		private IResponse response;

		[SetUp]
		public void Setup()
		{
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));
			requestMock = new DynamicMock(typeof(ICruiseRequest));

			buildLogAction = new HtmlBuildLogAction((IBuildRetriever) buildRetrieverMock.MockInstance, 
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ICruiseUrlBuilder) urlBuilderMock.MockInstance);

			buildLog = "some stuff in a log with a < and >";
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("myserver"), "myproject"), "mybuild");
			build = new Build(buildSpecifier, buildLog);
			response = new HtmlFragmentResponse("foo");
		}

		private void VerifyAll()
		{
			requestMock.Verify();
			buildRetrieverMock.Verify();
			velocityViewGeneratorMock.Verify();
			urlBuilderMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			requestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier);
			urlBuilderMock.ExpectAndReturn("Extension", "foo");
			urlBuilderMock.Expect("Extension", "xml");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "myUrl", XmlBuildLogAction.ACTION_NAME, buildSpecifier);
			urlBuilderMock.Expect("Extension", "foo");

			Hashtable expectedContext = new Hashtable();
			expectedContext["log"] = "some stuff in a log with a &lt; and &gt;";
			expectedContext["logUrl"] = "myUrl";

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", response, "BuildLog.vm", new HashtableConstraint(expectedContext));

			// Execute & Verify
			Assert.AreEqual(response, buildLogAction.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}
	}
}
