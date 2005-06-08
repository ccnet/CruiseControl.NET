using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.GenericPlugins
{
	[TestFixture]
	public class XslReportPluginTest
	{
		private DynamicMock actionInstantiatorMock;
		private XslReportBuildPlugin buildPlugin;

		[SetUp]
		public void Setup()
		{
			actionInstantiatorMock = new DynamicMock(typeof(IActionInstantiator));
			buildPlugin = new XslReportBuildPlugin((IActionInstantiator) actionInstantiatorMock.MockInstance);
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
			buildPlugin.XslFileName = @"xsl\myxsl.xsl";

			Assert.AreEqual("MyAction", buildPlugin.ActionName);
			Assert.AreEqual("My Plugin", buildPlugin.LinkDescription);
			Assert.AreEqual(@"xsl\myxsl.xsl", buildPlugin.XslFileName);

			VerifyAll();
		}

		[Test]
		public void ShouldCreateAnXslReportActionWithCorrectNameXslFileName()
		{
			buildPlugin.ActionName = "MyAction";
			buildPlugin.ConfiguredLinkDescription = "My Plugin";
			buildPlugin.XslFileName = @"xsl\myxsl.xsl";

			XslReportBuildAction xslReportAction = new XslReportBuildAction(null);
			actionInstantiatorMock.ExpectAndReturn("InstantiateAction", xslReportAction, typeof(XslReportBuildAction));

			INamedAction[] namedActions = buildPlugin.NamedActions;

			Assert.AreEqual(1, namedActions.Length);
			Assert.AreEqual("MyAction", namedActions[0].ActionName);
			Assert.AreEqual(xslReportAction, namedActions[0].Action);
			Assert.AreEqual(@"xsl\myxsl.xsl", ((XslReportBuildAction) namedActions[0].Action).XslFileName);

			VerifyAll();
		}
	}
}
