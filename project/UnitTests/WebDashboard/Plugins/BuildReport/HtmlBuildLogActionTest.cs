using System;
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
        private DynamicMock fingerprintFactoryMock;

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
		    fingerprintFactoryMock = new DynamicMock(typeof (IFingerprintFactory));

			buildLogAction = new HtmlBuildLogAction((IBuildRetriever) buildRetrieverMock.MockInstance, 
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ICruiseUrlBuilder) urlBuilderMock.MockInstance, 
                (IFingerprintFactory) fingerprintFactoryMock.MockInstance,
                null);

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
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, buildSpecifier, null);
			urlBuilderMock.ExpectAndReturn("Extension", "foo");
			urlBuilderMock.Expect("Extension", "xml");
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "myUrl", XmlBuildLogAction.ACTION_NAME, buildSpecifier);
			urlBuilderMock.Expect("Extension", "foo");

			Hashtable expectedContext = new Hashtable();
			expectedContext["log"] = "some stuff in a log with a &lt; and &gt;";
            expectedContext["ShowHighLight"] = false;
            expectedContext["logUrl"] = "myUrl";
            
			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", response, "BuildLog.vm", new HashtableConstraint(expectedContext));

			// Execute & Verify
			Assert.AreEqual(response, buildLogAction.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}

	    [Test]
        [Ignore("Difficult to mock because there is not a simple way to change a build name in to a build date.")]
	    public void ShouldReturnFingerprintBasedOnLogFileDateAndTemplateDate()
	    {
	        const string TEST_TOKEN = "test token";
            DateTime logFileDate = new DateTime(2006,12,2,1,4,5);
            DateTime templateDate = new DateTime(2005,1,2);

            DynamicMock requestMock = new DynamicMock(typeof(IRequest));
            requestMock.SetupResult("SubFolders", new string[] { "server", "testServer", "project", "testProject", "build", "testBuild" });

	        ConditionalGetFingerprint expectedFingerprint = new ConditionalGetFingerprint(logFileDate, TEST_TOKEN);

            Assert.AreEqual(expectedFingerprint, buildLogAction.GetFingerprint((IRequest) requestMock.MockInstance));
	    }
	}
}
