using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildPluginsTest
	{
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private BuildPlugins buildPlugins;
		private DynamicMock buildLinkFactoryMock;
		private DynamicMock configurationGetterMock;
		private DynamicMock objectGiverMock;

		[SetUp]
		public void Setup()
		{
			buildLinkFactoryMock = new DynamicMock(typeof(IBuildLinkFactory));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			objectGiverMock = new DynamicMock(typeof(ObjectGiver));
			buildPlugins = new BuildPlugins((IBuildLinkFactory) buildLinkFactoryMock.MockInstance, (IConfigurationGetter) configurationGetterMock.MockInstance, (ObjectGiver) objectGiverMock.MockInstance);
		}

		private void VerifyAll()
		{
			buildLinkFactoryMock.Verify();
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

			linkRendererMock1.ExpectAndReturn("Description", "Description 1");
			linkRendererMock1.ExpectAndReturn("ActionName", "Action Name 1");
			linkRendererMock2.ExpectAndReturn("Description", "Description 2");
			linkRendererMock2.ExpectAndReturn("ActionName", "Action Name 2");

			IAbsoluteLink link1 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;
			IAbsoluteLink link2 = (IAbsoluteLink) new DynamicMock(typeof(IAbsoluteLink)).MockInstance;

			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", link1, serverName, projectName, buildName, "Description 1", new PropertyIs("ActionName", "Action Name 1"));
			buildLinkFactoryMock.ExpectAndReturn("CreateBuildLink", link2, serverName, projectName, buildName, "Description 2", new PropertyIs("ActionName", "Action Name 2"));

			IAbsoluteLink[] buildLinks = buildPlugins.GetBuildPluginLinks(serverName, projectName, buildName);

			Assert.AreSame(link1, buildLinks[0]);
			Assert.AreSame(link2, buildLinks[1]);

			Assert.AreEqual(2, buildLinks.Length);

			VerifyAll();
		}
	}
}
