using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class MultipleXslReportActionTest
	{
		[Test]
		public void ShouldUseBuildLogTransformerToGenerateView()
		{
			var buildLogTransformerMock = new Mock<IBuildLogTransformer>();
			var cruiseRequestMock = new Mock<ICruiseRequest>();
			var buildSpecifierMock = new Mock<IBuildSpecifier>();
			var requestStub = new Mock<IRequest>();

			ICruiseRequest cruiseRequest = (ICruiseRequest) cruiseRequestMock.Object;
			IBuildSpecifier buildSpecifier = (IBuildSpecifier) buildSpecifierMock.Object;
			IRequest request = (IRequest) requestStub.Object;

			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildSpecifier).Returns(buildSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request).Verifiable();
			requestStub.SetupGet(_request => _request.ApplicationPath).Returns("myAppPath").Verifiable();
			buildLogTransformerMock.Setup(transformer => transformer.Transform(buildSpecifier, new string[] { @"xsl\myxsl.xsl", @"xsl\myotherxsl.xsl" }, It.Is<Hashtable>(t => t.Count == 1 && (string)t["applicationPath"] == "myAppPath"), null)).
				Returns("transformed").Verifiable();

			MultipleXslReportBuildAction buildAction = new MultipleXslReportBuildAction((IBuildLogTransformer) buildLogTransformerMock.Object, null);
            buildAction.XslFileNames = new BuildReportXslFilename[] { 
                new BuildReportXslFilename(@"xsl\myxsl.xsl"),
                new BuildReportXslFilename(@"xsl\myotherxsl.xsl")
            };

			Assert.AreEqual("transformed", ((HtmlFragmentResponse) buildAction.Execute(cruiseRequest)).ResponseFragment);

			buildLogTransformerMock.Verify();
			cruiseRequestMock.Verify();
			buildSpecifierMock.Verify();
		}
	}
}
