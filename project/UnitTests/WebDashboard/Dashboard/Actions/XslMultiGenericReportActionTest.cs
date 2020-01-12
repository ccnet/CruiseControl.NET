using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard.Actions
{
	[TestFixture]
	public class XslMultiGenericReportActionTest
	{
		private Mock<IActionInstantiator> actionInstantiatorMock;
		private XslMultiReportBuildPlugin buildPlugin;
        private BuildReportXslFilename[] xslFiles = new BuildReportXslFilename[] {
            new BuildReportXslFilename(@"xsl\myxsl.xsl"),
            new BuildReportXslFilename(@"xsl\myxsl2.xsl")
        };

		[SetUp]
		public void Setup()
		{
			actionInstantiatorMock = new Mock<IActionInstantiator>();
			buildPlugin = new XslMultiReportBuildPlugin((IActionInstantiator) actionInstantiatorMock.Object);
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

			MultipleXslReportBuildAction xslReportAction = new MultipleXslReportBuildAction(null, null);
			actionInstantiatorMock.Setup(instantiator => instantiator.InstantiateAction(typeof(MultipleXslReportBuildAction))).Returns(xslReportAction).Verifiable();

			INamedAction[] namedActions = buildPlugin.NamedActions;

			Assert.AreEqual(1, namedActions.Length);
			Assert.AreEqual("MyAction", namedActions[0].ActionName);
			Assert.AreEqual(xslReportAction, namedActions[0].Action);
			Assert.AreEqual(xslFiles, ((MultipleXslReportBuildAction) namedActions[0].Action).XslFileNames);

			VerifyAll();
		}
	}
}