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
		private XslReportPlugin plugin;

		[SetUp]
		public void Setup()
		{
			actionInstantiatorMock = new DynamicMock(typeof(IActionInstantiator));
			plugin = new XslReportPlugin((IActionInstantiator) actionInstantiatorMock.MockInstance);
		}

		private void VerifyAll()
		{
			actionInstantiatorMock.Verify();
		}

		[Test]
		public void ShouldUseConfigurableProperties()
		{
			plugin.ActionName = "MyAction";
			plugin.LinkDescription = "My Plugin";
			plugin.XslFileName = @"xsl\myxsl.xsl";

			Assert.AreEqual("MyAction", plugin.ActionName);
			Assert.AreEqual("My Plugin", plugin.LinkDescription);
			Assert.AreEqual(@"xsl\myxsl.xsl", plugin.XslFileName);

			VerifyAll();
		}

		[Test]
		public void ShouldCreateAnXslReportActionWithCorrectNameXslFileName()
		{
			plugin.ActionName = "MyAction";
			plugin.LinkDescription = "My Plugin";
			plugin.XslFileName = @"xsl\myxsl.xsl";

			XslReportAction xslReportAction = new XslReportAction(null);
			actionInstantiatorMock.ExpectAndReturn("InstantiateAction", xslReportAction, typeof(XslReportAction));

			INamedAction[] namedActions = plugin.NamedActions;

			Assert.AreEqual(1, namedActions.Length);
			Assert.AreEqual("MyAction", namedActions[0].ActionName);
			Assert.AreEqual(xslReportAction, namedActions[0].Action);
			Assert.AreEqual(@"xsl\myxsl.xsl", ((XslReportAction) namedActions[0].Action).XslFileName);

			VerifyAll();
		}
	}
}
