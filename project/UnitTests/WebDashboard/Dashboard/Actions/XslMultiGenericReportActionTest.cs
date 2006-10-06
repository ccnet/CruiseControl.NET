using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class XslMultiGenericReportActionTest
	{
		private DynamicMock actionInstantiatorMock;
		private XslMultiReportBuildPlugin buildPlugin;
		private string[] xslFiles = new string[2]{@"xsl\myxsl.xsl", @"xsl\myxsl2.xsl"};

		[SetUp]
		public void Setup()
		{
			actionInstantiatorMock = new DynamicMock(typeof(IActionInstantiator));
			buildPlugin = new XslMultiReportBuildPlugin((IActionInstantiator) actionInstantiatorMock.MockInstance);
		}

		private void VerifyAll()
		{
			actionInstantiatorMock.Verify();
		}

		[Test]
		public void ShouldUseConfigurableProperties()
		{
			buildPlugin.ActionName = "MyAction";
			buildPlugin.ConfiguredLinkDescription = "My Plugin";
			buildPlugin.XslFileNames = xslFiles;

			Assert.AreEqual("MyAction", buildPlugin.ActionName);
			Assert.AreEqual("My Plugin", buildPlugin.LinkDescription);
			Assert.AreEqual(xslFiles, buildPlugin.XslFileNames);

			VerifyAll();
		}

		[Test]
		public void ShouldCreateAnMultipleXslReportBuildActionWithCorrectNameXslFileName()
		{
			buildPlugin.ActionName = "MyAction";
			buildPlugin.ConfiguredLinkDescription = "My Plugin";
			buildPlugin.XslFileNames = xslFiles;

			MultipleXslReportBuildAction xslReportAction = new MultipleXslReportBuildAction(null);
			actionInstantiatorMock.ExpectAndReturn("InstantiateAction", xslReportAction, typeof(MultipleXslReportBuildAction));

			INamedAction[] namedActions = buildPlugin.NamedActions;

			Assert.AreEqual(1, namedActions.Length);
			Assert.AreEqual("MyAction", namedActions[0].ActionName);
			Assert.AreEqual(xslReportAction, namedActions[0].Action);
			Assert.AreEqual(xslFiles, ((MultipleXslReportBuildAction) namedActions[0].Action).XslFileNames);

			VerifyAll();
		}
	}
}
