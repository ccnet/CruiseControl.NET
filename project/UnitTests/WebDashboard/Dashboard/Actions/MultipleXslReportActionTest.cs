using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class MultipleXslReportActionTest
	{
		[Test]
		public void ShouldUseBuildLogTransformerToGenerateView()
		{
			DynamicMock buildLogTransformerMock = new DynamicMock(typeof(IBuildLogTransformer));
			DynamicMock cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			DynamicMock buildSpecifierMock = new DynamicMock(typeof(IBuildSpecifier));
			DynamicMock requestStub = new DynamicMock(typeof(IRequest));

			ICruiseRequest cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
			IBuildSpecifier buildSpecifier = (IBuildSpecifier) buildSpecifierMock.MockInstance;
			IRequest request = (IRequest) requestStub.MockInstance;

			cruiseRequestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			cruiseRequestMock.SetupResult("Request", request);
			requestStub.SetupResult("ApplicationPath", "myAppPath");
			Hashtable expectedXsltArgs = new Hashtable();
			expectedXsltArgs["applicationPath"] = "myAppPath";
			buildLogTransformerMock.ExpectAndReturn("Transform", "transformed", buildSpecifier, new string[] { @"xsl\myxsl.xsl", @"xsl\myotherxsl.xsl" }, new HashtableConstraint(expectedXsltArgs));

			MultipleXslReportBuildAction buildAction = new MultipleXslReportBuildAction((IBuildLogTransformer) buildLogTransformerMock.MockInstance, null);
			buildAction.XslFileNames = new string[] { @"xsl\myxsl.xsl", @"xsl\myotherxsl.xsl" };

			Assert.AreEqual("transformed", ((HtmlFragmentResponse) buildAction.Execute(cruiseRequest)).ResponseFragment);

			buildLogTransformerMock.Verify();
			cruiseRequestMock.Verify();
			buildSpecifierMock.Verify();
		}
	}
}
