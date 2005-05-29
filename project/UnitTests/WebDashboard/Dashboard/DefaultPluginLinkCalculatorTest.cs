using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultPluginLinkCalculatorTest
	{
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private DefaultPluginLinkCalculator Plugins;
		private DynamicMock linkFactoryMock;
		private DynamicMock configurationMock;
		private DefaultBuildSpecifier buildSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultServerSpecifier serverSpecifier;
		private DynamicMock pluginMock1;
		private DynamicMock pluginMock2;
		private INamedAction action1;
		private INamedAction action2;
		private IAbsoluteLink link1;
		private IAbsoluteLink link2;

		[SetUp]
		public void Setup()
		{
			serverSpecifier = new DefaultServerSpecifier(serverName);
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, projectName);
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, buildName);
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			configurationMock = new DynamicMock(typeof(IPluginConfiguration));
			Plugins = new DefaultPluginLinkCalculator((ILinkFactory) linkFactoryMock.MockInstance, (IPluginConfiguration) configurationMock.MockInstance);

			pluginMock1 = new DynamicMock(typeof(IPlugin));
			pluginMock2 = new DynamicMock(typeof(IPlugin));
			action1 = new ImmutableNamedAction("Action Name 1", null);
			action2 = new ImmutableNamedAction("Action Name 2", null);
			pluginMock1.ExpectAndReturn("LinkDescription", "Description 1");
			pluginMock1.ExpectAndReturn("NamedActions", new INamedAction[] { action1 } );
			pluginMock2.ExpectAndReturn("LinkDescription", "Description 2");
			pluginMock2.ExpectAndReturn("NamedActions", new INamedAction[] { action2 });
			link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
			configurationMock.Verify();
		}

		[Test]
		public void ShouldReturnBuildPluginLinksByQueryingConfiguration()
		{
			configurationMock.ExpectAndReturn("BuildPlugins", new IPlugin[] { (IPlugin) pluginMock1.MockInstance, (IPlugin) pluginMock2.MockInstance });
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", link1, buildSpecifier, "Description 1", "Action Name 1");
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", link2, buildSpecifier, "Description 2", "Action Name 2");

			IAbsoluteLink[] buildLinks = Plugins.GetBuildPluginLinks(buildSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnServerPluginLinksByQueryingConfiguration()
		{
			configurationMock.ExpectAndReturn("ServerPlugins", new IPlugin[] { (IPlugin) pluginMock1.MockInstance, (IPlugin) pluginMock2.MockInstance });
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link1, serverSpecifier, "Description 1", "Action Name 1");
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier, "Description 2", "Action Name 2");

			IAbsoluteLink[] buildLinks = Plugins.GetServerPluginLinks(serverSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnFarmPluginLinksByQueryingConfiguration()
		{
			configurationMock.ExpectAndReturn("FarmPlugins", new IPlugin[] { (IPlugin) pluginMock1.MockInstance, (IPlugin) pluginMock2.MockInstance });
			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Description 1", "Action Name 1");
			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link2, "Description 2", "Action Name 2");

			IAbsoluteLink[] buildLinks = Plugins.GetFarmPluginLinks();

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectPluginLinksByQueryingConfiguration()
		{
			configurationMock.ExpectAndReturn("ProjectPlugins", new IPlugin[] { (IPlugin) pluginMock1.MockInstance, (IPlugin) pluginMock2.MockInstance });
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link1, projectSpecifier, "Description 1", "Action Name 1");
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link2, projectSpecifier, "Description 2", "Action Name 2");

			IAbsoluteLink[] buildLinks = Plugins.GetProjectPluginLinks(projectSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}
	}
}
