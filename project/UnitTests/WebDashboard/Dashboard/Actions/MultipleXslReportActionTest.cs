using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

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

			ICruiseRequest cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
			IBuildSpecifier buildSpecifier = (IBuildSpecifier) buildSpecifierMock.MockInstance;

			cruiseRequestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);
			buildLogTransformerMock.ExpectAndReturn("Transform", "transformed", buildSpecifier, new string[] { @"xsl\myxsl.xsl", @"xsl\myotherxsl.xsl" });

			MultipleXslReportAction action = new MultipleXslReportAction((IBuildLogTransformer) buildLogTransformerMock.MockInstance);
			action.XslFileNames = new string[] { @"xsl\myxsl.xsl", @"xsl\myotherxsl.xsl" };

			Assert.AreEqual("transformed", action.Execute(cruiseRequest).ResponseFragment);

			buildLogTransformerMock.Verify();
			cruiseRequestMock.Verify();
			buildSpecifierMock.Verify();
		}
	}
}
