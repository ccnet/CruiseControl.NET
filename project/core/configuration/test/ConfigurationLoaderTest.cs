using System;
using System.Xml;

using Exortech.NetReflector;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Publishers.Test;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Config.Test
{
	[TestFixture]
	public class ConfigurationLoaderTest : CustomAssertion
	{
		private ConfigurationLoader loader;
		
		[SetUp]
		protected void SetUp()
		{
			loader = new ConfigurationLoader();
		}

		[TearDown]
		protected void TearDown() 
		{
			TempFileUtil.DeleteTempDir(this);
		}

		[Test]
		public void LoadConfigurationFile()
		{
			string xml = "<cruisecontrol></cruisecontrol>";
			loader.ConfigFile = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config", xml);
			XmlDocument config = loader.LoadConfiguration();
			AssertNotNull("config file should not be null", config);
			AssertEquals(xml, config.OuterXml);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfigurationFile_MissingFile()
		{
			loader.ConfigFile = @"c:\unknown\config.file.xml";
			loader.LoadConfiguration();
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfigurationFile_FileOnlyNoPath()
		{
			loader.ConfigFile = @"ccnet_unknown.config";
			loader.LoadConfiguration();
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void LoadConfiguration_BadXml()
		{
			loader.ConfigFile = TempFileUtil.CreateTempXmlFile(TempFileUtil.CreateTempDir(this), "loadernet.config"
				, "<test><a><b/></test>");
			XmlDocument config = loader.LoadConfiguration();
		}

		[Test]
		public void PopulateProjectsFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

		[Test]
		public void PopulateProjectsFromXml_TwoProjects()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			string project2Xml = ConfigurationFixture.GenerateProjectXml("test2");
			IConfiguration configuration = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml + project2Xml));
			ValidateProject(configuration, "test");
			ValidateProject(configuration, "test2");
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void Populate_MissingProjectProperties()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml(null, null, null, null, null, null);
			loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(projectXml));
		}

		private void ValidateProject(IConfiguration configuration, string projectName)
		{
			Project project = configuration.Projects[projectName] as Project;
			AssertEquals(projectName, project.Name);
			AssertEquals(typeof(MockBuilder), project.Builder);
			AssertEquals(typeof(DefaultSourceControl), project.SourceControl);
			AssertEquals("missing publisher", 1, project.Publishers.Length);
			AssertEquals(typeof(MockPublisher), project.Publishers[0]);

		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_EmptyDocument()
		{
			loader.PopulateProjectsFromXml(new XmlDocument());
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateProjectsFromXml_InvalidRootElement()
		{
			loader.PopulateProjectsFromXml(XmlUtil.CreateDocument("<loader/>"));
		}

		[Test]
		public void PopulateCustomProjectFromXml()
		{
			string xml = @"<customtestproject name=""foo"" />";
			IConfiguration configuration = loader.PopulateProjectsFromXml(ConfigurationFixture.GenerateConfig(xml));
			AssertNotNull(configuration.Projects["foo"]);
			AssertEquals(typeof(CustomTestProject), configuration.Projects["foo"]);
			AssertEquals("foo", ((CustomTestProject) configuration.Projects["foo"]).Name);
		}

		[ReflectorType("customtestproject")]
		class CustomTestProject : ProjectBase, IProject
		{
			public ProjectActivity CurrentActivity { get { return ProjectActivity.Building; } }
			public IntegrationResult RunIntegration(BuildCondition buildCondition) { return null; }
			public IntegrationStatus GetLatestBuildStatus() { return IntegrationStatus.Success; }
			public string WebURL { get {return ""; }	}
		}
	}
}
