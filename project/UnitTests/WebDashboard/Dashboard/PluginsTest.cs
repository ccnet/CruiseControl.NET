using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class PluginsTest
	{
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private CruiseControl.WebDashboard.Dashboard.Plugins Plugins;
		private DynamicMock linkFactoryMock;
		private DynamicMock configurationGetterMock;
		private DynamicMock objectGiverMock;
		private DefaultBuildSpecifier buildSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultServerSpecifier serverSpecifier;

		[SetUp]
		public void Setup()
		{
			serverSpecifier = new DefaultServerSpecifier(serverName);
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, projectName);
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, buildName);
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			objectGiverMock = new DynamicMock(typeof(ObjectGiver));
			Plugins = new CruiseControl.WebDashboard.Dashboard.Plugins((ILinkFactory) linkFactoryMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance, (ObjectGiver) objectGiverMock.MockInstance);
		}

		private void VerifyAll()
		{
			linkFactoryMock.Verify();
			configurationGetterMock.Verify();
			objectGiverMock.Verify();
		}

		[Test]
		public void ShouldReturnBuildPluginLinksByQueryingConfiguration()
		{
			DynamicMock pluginSpecificationMock1 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock pluginSpecificationMock2 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock linkRendererMock1 = new DynamicMock(typeof(IPluginLinkRenderer));
			DynamicMock linkRendererMock2 = new DynamicMock(typeof(IPluginLinkRenderer));

			IPluginSpecification[] pluginSpecs = new IPluginSpecification[] { (IPluginSpecification) pluginSpecificationMock1.MockInstance, (IPluginSpecification) pluginSpecificationMock2.MockInstance };

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", pluginSpecs, "CCNet/buildPlugins");

			pluginSpecificationMock1.ExpectAndReturn("Type", typeof(string));
			pluginSpecificationMock2.ExpectAndReturn("Type", typeof(int));

			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock1.MockInstance, typeof(string));
			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock2.MockInstance, typeof(int));

			linkRendererMock1.ExpectAndReturn("LinkDescription", "Description 1");
			linkRendererMock1.ExpectAndReturn("LinkActionName", "Action Name 1");
			linkRendererMock2.ExpectAndReturn("LinkDescription", "Description 2");
			linkRendererMock2.ExpectAndReturn("LinkActionName", "Action Name 2");

			IAbsoluteLink link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			linkFactoryMock.ExpectAndReturn("CreateBuildLink", link1, buildSpecifier, "Description 1", new PropertyIs("ActionName", "Action Name 1"));
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", link2, buildSpecifier, "Description 2", new PropertyIs("ActionName", "Action Name 2"));

			IAbsoluteLink[] buildLinks = Plugins.GetBuildPluginLinks(buildSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);

			Assert.AreEqual(2, buildLinks.Length);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnServerPluginLinksByQueryingConfiguration()
		{
			DynamicMock pluginSpecificationMock1 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock pluginSpecificationMock2 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock linkRendererMock1 = new DynamicMock(typeof(IPluginLinkRenderer));
			DynamicMock linkRendererMock2 = new DynamicMock(typeof(IPluginLinkRenderer));

			IPluginSpecification[] pluginSpecs = new IPluginSpecification[] { (IPluginSpecification) pluginSpecificationMock1.MockInstance, (IPluginSpecification) pluginSpecificationMock2.MockInstance };

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", pluginSpecs, "CCNet/serverPlugins");

			pluginSpecificationMock1.ExpectAndReturn("Type", typeof(string));
			pluginSpecificationMock2.ExpectAndReturn("Type", typeof(int));

			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock1.MockInstance, typeof(string));
			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock2.MockInstance, typeof(int));

			linkRendererMock1.ExpectAndReturn("LinkDescription", "Description 1");
			linkRendererMock1.ExpectAndReturn("LinkActionName", "Action Name 1");
			linkRendererMock2.ExpectAndReturn("LinkDescription", "Description 2");
			linkRendererMock2.ExpectAndReturn("LinkActionName", "Action Name 2");

			IAbsoluteLink link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			linkFactoryMock.ExpectAndReturn("CreateServerLink", link1, serverSpecifier, "Description 1", new PropertyIs("ActionName", "Action Name 1"));
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier, "Description 2", new PropertyIs("ActionName", "Action Name 2"));

			IAbsoluteLink[] buildLinks = Plugins.GetServerPluginLinks(serverSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);

			Assert.AreEqual(2, buildLinks.Length);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnFarmPluginLinksByQueryingConfiguration()
		{
			DynamicMock pluginSpecificationMock1 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock pluginSpecificationMock2 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock linkRendererMock1 = new DynamicMock(typeof(IPluginLinkRenderer));
			DynamicMock linkRendererMock2 = new DynamicMock(typeof(IPluginLinkRenderer));

			IPluginSpecification[] pluginSpecs = new IPluginSpecification[] { (IPluginSpecification) pluginSpecificationMock1.MockInstance, (IPluginSpecification) pluginSpecificationMock2.MockInstance };

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", pluginSpecs, "CCNet/farmPlugins");

			pluginSpecificationMock1.ExpectAndReturn("Type", typeof(string));
			pluginSpecificationMock2.ExpectAndReturn("Type", typeof(int));

			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock1.MockInstance, typeof(string));
			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock2.MockInstance, typeof(int));

			linkRendererMock1.ExpectAndReturn("LinkDescription", "Description 1");
			linkRendererMock1.ExpectAndReturn("LinkActionName", "Action Name 1");
			linkRendererMock2.ExpectAndReturn("LinkDescription", "Description 2");
			linkRendererMock2.ExpectAndReturn("LinkActionName", "Action Name 2");

			IAbsoluteLink link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Description 1", new PropertyIs("ActionName", "Action Name 1"));
			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link2, "Description 2", new PropertyIs("ActionName", "Action Name 2"));

			IAbsoluteLink[] buildLinks = Plugins.GetFarmPluginLinks();

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);

			Assert.AreEqual(2, buildLinks.Length);

			VerifyAll();
		}

		[Test]
		public void ShouldReturnProjectPluginLinksByQueryingConfiguration()
		{
			DynamicMock pluginSpecificationMock1 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock pluginSpecificationMock2 = new DynamicMock(typeof(IPluginSpecification));
			DynamicMock linkRendererMock1 = new DynamicMock(typeof(IPluginLinkRenderer));
			DynamicMock linkRendererMock2 = new DynamicMock(typeof(IPluginLinkRenderer));

			IPluginSpecification[] pluginSpecs = new IPluginSpecification[] { (IPluginSpecification) pluginSpecificationMock1.MockInstance, (IPluginSpecification) pluginSpecificationMock2.MockInstance };

			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", pluginSpecs, "CCNet/projectPlugins");

			pluginSpecificationMock1.ExpectAndReturn("Type", typeof(string));
			pluginSpecificationMock2.ExpectAndReturn("Type", typeof(int));

			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock1.MockInstance, typeof(string));
			objectGiverMock.ExpectAndReturn("GiveObjectByType", linkRendererMock2.MockInstance, typeof(int));

			linkRendererMock1.ExpectAndReturn("LinkDescription", "Description 1");
			linkRendererMock1.ExpectAndReturn("LinkActionName", "Action Name 1");
			linkRendererMock2.ExpectAndReturn("LinkDescription", "Description 2");
			linkRendererMock2.ExpectAndReturn("LinkActionName", "Action Name 2");

			IAbsoluteLink link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link1, projectSpecifier, "Description 1", new PropertyIs("ActionName", "Action Name 1"));
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link2, projectSpecifier, "Description 2", new PropertyIs("ActionName", "Action Name 2"));

			IAbsoluteLink[] buildLinks = Plugins.GetProjectPluginLinks(projectSpecifier);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);

			Assert.AreEqual(2, buildLinks.Length);

			VerifyAll();
		}
	}
}
