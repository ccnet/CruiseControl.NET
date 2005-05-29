using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class XslReportActionTest
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
			buildLogTransformerMock.ExpectAndReturn("Transform", "transformed", buildSpecifier, new string[] { @"xsl\myxsl.xsl" });

			XslReportBuildAction action = new XslReportBuildAction((IBuildLogTransformer) buildLogTransformerMock.MockInstance);
			action.XslFileName = @"xsl\myxsl.xsl";

			Assert.AreEqual("transformed", action.Execute(cruiseRequest).ResponseFragment);

			buildLogTransformerMock.Verify();
			cruiseRequestMock.Verify();
			buildSpecifierMock.Verify();
		}
	}
}
