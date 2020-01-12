using Moq;
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
		private Mock<ILinkFactory> linkFactoryMock;
		private Mock<IPluginConfiguration> configurationMock;
		private DefaultBuildSpecifier buildSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultServerSpecifier serverSpecifier;
		private Mock<IPlugin> pluginMock1;
		private Mock<IPlugin> pluginMock2;
		private INamedAction action1;
		private INamedAction action2;
		private INamedAction action3;
		private IAbsoluteLink link1;
		private IAbsoluteLink link2;

		[SetUp]
		public void Setup()
		{
			serverSpecifier = new DefaultServerSpecifier(serverName);
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, projectName);
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, buildName);
			linkFactoryMock = new Mock<ILinkFactory>();
			configurationMock = new Mock<IPluginConfiguration>();
			Plugins = new DefaultPluginLinkCalculator((ILinkFactory) linkFactoryMock.Object, (IPluginConfiguration) configurationMock.Object);

			pluginMock1 = new Mock<IPlugin>();
			pluginMock2 = new Mock<IPlugin>();
			action1 = new ImmutableNamedAction("Action Name 1", null);
			action2 = new ImmutableNamedAction("Action Name 2", null);
			action3 = new ImmutableNamedAction("Action Name 3", null);
			pluginMock1.SetupGet(plugin => plugin.LinkDescription).Returns("Description 1").Verifiable();
			pluginMock1.SetupGet(plugin => plugin.NamedActions).Returns(new INamedAction[] {action1}).Verifiable();
			pluginMock2.SetupGet(plugin => plugin.LinkDescription).Returns("Description 2").Verifiable();
			pluginMock2.SetupGet(plugin => plugin.NamedActions).Returns(new INamedAction[] {action2}).Verifiable();
			link1 = (IAbsoluteLink) new Mock<IAbsoluteLink>().Object;
			link2 = (IAbsoluteLink) new Mock<IAbsoluteLink>().Object;
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
			configurationMock.Verify();
		}

		[Test]
		public void ShouldReturnBuildPluginLinksRelevantToThisProject()
		{
			var buildPluginMock1 = new Mock<IBuildPlugin>();
			var buildPluginMock2 = new Mock<IBuildPlugin>();
			var buildPluginMock3 = new Mock<IBuildPlugin>();
			buildPluginMock1.SetupGet(plugin => plugin.LinkDescription).Returns("Description 1");
			buildPluginMock1.SetupGet(plugin => plugin.NamedActions).Returns(new INamedAction[] {action1});
			buildPluginMock1.Setup(plugin => plugin.IsDisplayedForProject(It.IsAny<IProjectSpecifier>())).Returns(true);
			buildPluginMock2.SetupGet(plugin => plugin.LinkDescription).Returns("Description 2");
			buildPluginMock2.SetupGet(plugin => plugin.NamedActions).Returns(new INamedAction[] {action2});
			buildPluginMock2.Setup(plugin => plugin.IsDisplayedForProject(It.IsAny<IProjectSpecifier>())).Returns(true);
			buildPluginMock3.Setup(plugin => plugin.IsDisplayedForProject(It.IsAny<IProjectSpecifier>())).Returns(false);

			configurationMock.SetupGet(_configuration => _configuration.BuildPlugins).Returns(new IBuildPlugin[]
				{
					(IBuildPlugin) buildPluginMock1.Object, (IBuildPlugin) buildPluginMock2.Object, (IBuildPlugin) buildPluginMock3.Object
				}).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateBuildLink(buildSpecifier, "Description 1", "Action Name 1")).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateBuildLink(buildSpecifier, "Description 2", "Action Name 2")).Returns(link2).Verifiable();

			IAbsoluteLink[] buildLinks = Plugins.GetBuildPluginLinks(buildSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnServerPluginLinksByQueryingConfiguration()
		{
			configurationMock.SetupGet(_configuration => _configuration.ServerPlugins).Returns(new IPlugin[] { (IPlugin)pluginMock1.Object, (IPlugin)pluginMock2.Object }).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, "Description 1", "Action Name 1")).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, "Description 2", "Action Name 2")).Returns(link2).Verifiable();

			IAbsoluteLink[] buildLinks = Plugins.GetServerPluginLinks(serverSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnFarmPluginLinksByQueryingConfiguration()
		{
			configurationMock.SetupGet(_configuration => _configuration.FarmPlugins).Returns(new IPlugin[] { (IPlugin)pluginMock1.Object, (IPlugin)pluginMock2.Object }).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Description 1", "Action Name 1")).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Description 2", "Action Name 2")).Returns(link2).Verifiable();

			IAbsoluteLink[] buildLinks = Plugins.GetFarmPluginLinks();

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectPluginLinksByQueryingConfiguration()
		{
			configurationMock.SetupGet(_configuration => _configuration.ProjectPlugins).Returns(new IPlugin[] { (IPlugin)pluginMock1.Object, (IPlugin)pluginMock2.Object }).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, "Description 1", "Action Name 1")).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, "Description 2", "Action Name 2")).Returns(link2).Verifiable();

			IAbsoluteLink[] buildLinks = Plugins.GetProjectPluginLinks(projectSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);
			Assert.AreEqual(2, buildLinks.Length);
			VerifyAll();
		}
	}
}