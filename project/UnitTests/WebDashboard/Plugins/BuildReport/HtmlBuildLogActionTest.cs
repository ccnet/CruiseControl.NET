using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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

		private Mock<ICruiseRequest> requestMock;
		private Mock<IBuildRetriever> buildRetrieverMock;
		private Mock<ICruiseUrlBuilder> urlBuilderMock;
		private Mock<IVelocityViewGenerator> velocityViewGeneratorMock;
        private Mock<IFingerprintFactory> fingerprintFactoryMock;

		private string buildLog;
		private Build build;
		private DefaultBuildSpecifier buildSpecifier;
		private HtmlFragmentResponse response;

	    [SetUp]
		public void Setup()
		{
			buildRetrieverMock = new Mock<IBuildRetriever>();
			velocityViewGeneratorMock = new Mock<IVelocityViewGenerator>();
			urlBuilderMock = new Mock<ICruiseUrlBuilder>();
			requestMock = new Mock<ICruiseRequest>();
		    fingerprintFactoryMock = new Mock<IFingerprintFactory>();

			buildLogAction = new HtmlBuildLogAction((IBuildRetriever) buildRetrieverMock.Object, 
				(IVelocityViewGenerator) velocityViewGeneratorMock.Object,
				(ICruiseUrlBuilder) urlBuilderMock.Object, 
                (IFingerprintFactory) fingerprintFactoryMock.Object,
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
			requestMock.SetupGet(request => request.BuildSpecifier).Returns(buildSpecifier).Verifiable();
			buildRetrieverMock.Setup(retriever => retriever.GetBuild(buildSpecifier, null)).Returns(build).Verifiable();
			urlBuilderMock.SetupGet(builder => builder.Extension).Returns("foo").Verifiable();
			urlBuilderMock.SetupSet(builder => builder.Extension = "xml").Verifiable();
			urlBuilderMock.Setup(builder => builder.BuildBuildUrl(XmlBuildLogAction.ACTION_NAME, buildSpecifier)).Returns("myUrl").Verifiable();
			urlBuilderMock.SetupSet(builder => builder.Extension = "foo").Verifiable();

			Hashtable expectedContext = new Hashtable();
			expectedContext["log"] = "some stuff in a log with a &lt; and &gt;";
            expectedContext["ShowHighLight"] = false;
            expectedContext["logUrl"] = "myUrl";
            
			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"BuildLog.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedContext)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, buildLogAction.Execute((ICruiseRequest) requestMock.Object));

			VerifyAll();
		}

	    [Test]
        [Ignore("Difficult to mock because there is not a simple way to change a build name in to a build date.")]
	    public void ShouldReturnFingerprintBasedOnLogFileDateAndTemplateDate()
	    {
	        const string TEST_TOKEN = "test token";
            DateTime logFileDate = new DateTime(2006,12,2,1,4,5);
            DateTime templateDate = new DateTime(2005,1,2);

            var requestMock = new Mock<IRequest>();
            requestMock.SetupGet(request => request.SubFolders).Returns(new string[] { "server", "testServer", "project", "testProject", "build", "testBuild" });

	        ConditionalGetFingerprint expectedFingerprint = new ConditionalGetFingerprint(logFileDate, TEST_TOKEN);

            Assert.AreEqual(expectedFingerprint, buildLogAction.GetFingerprint((IRequest) requestMock.Object));
	    }
	}
}
