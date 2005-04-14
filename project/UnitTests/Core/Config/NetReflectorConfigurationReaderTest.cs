using System.Collections;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Tasks.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class NetReflectorConfigurationReaderTest
	{
		private NetReflectorConfigurationReader reader;
		private IList unusedNodes;

		[SetUp]
		protected void CreateReader()
		{
			reader = new NetReflectorConfigurationReader();
			reader.UnusedNodeEventHandler += new UnusedNodeEventHandler(CheckUnusedNodes);
			unusedNodes = new ArrayList();
		}

		[Test]
		public void DeserialiseSingleProjectFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

		[Test]
		public void DeserialiseTwoProjectsFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			string project2Xml = ConfigurationFixture.GenerateProjectXml("test2");
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml + project2Xml));
			ValidateProject(configuration, "test");
			ValidateProject(configuration, "test2");
		}

		[Test] // [CCNET-63] XML comments before project tag was causing NetReflectorException
		public void DeserialiseSingleProjectFromXmlWithComments()
		{
			string projectXml = @"<!-- A Comment -->" + ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

		[Test]
		public void DeserialiseCustomProjectFromXml()
		{
			string xml = @"<customtestproject name=""foo"" />";
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(xml));
			Assert.IsNotNull(configuration.Projects["foo"]);
			Assert.IsTrue(configuration.Projects["foo"] is CustomTestProject);
			Assert.AreEqual("foo", ((CustomTestProject) configuration.Projects["foo"]).Name);
		}

		[Test]
		public void DeserialiseProjectFromXmlWithUnusedNodesShouldGenerateEvent()
		{
			string xml = @"<customtestproject name=""foo"" bar=""baz"" />";
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(xml));
			Assert.IsNotNull(configuration.Projects["foo"]);
			Assert.AreEqual(1, unusedNodes.Count);
			Assert.AreEqual("bar", ((XmlNode)unusedNodes[0]).Name);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void AttemptToDeserialiseProjectWithMissingXmlForRequiredProperties()
		{
			string projectXml = @"<project />";
			reader.Read(ConfigurationFixture.GenerateConfig(projectXml));
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void AttemptToDeserialiseProjectFromEmptyDocument()
		{
			reader.Read(new XmlDocument());
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void AttemptToDeserialiseProjectFromXmlWithInvalidRootElement()
		{
			reader.Read(XmlUtil.CreateDocument("<loader/>"));
		}
        
		private void ValidateProject(IConfiguration configuration, string projectName)
		{
			Project project = configuration.Projects[projectName] as Project;
			Assert.AreEqual(projectName, project.Name);
			Assert.IsTrue(project.Builder is MockBuilder);
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.AreEqual(1, project.Publishers.Length);
			Assert.IsTrue(project.Publishers[0] is MockPublisher);
			if (unusedNodes.Count > 0) 
				Assert.Fail("The xml contains nodes that are no longer used: {0}.", ((XmlNode)unusedNodes[0]).OuterXml);				
		}

		[ReflectorType("customtestproject")]
		class CustomTestProject : ProjectBase, IProject
		{
			public ProjectActivity CurrentActivity { get { return ProjectActivity.Building; } }
			public IIntegrationResult RunIntegration(BuildCondition buildCondition) { return null; }
			public IntegrationStatus LatestBuildStatus { get { return IntegrationStatus.Success; } }
			public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment) { }
			public string WebURL { get {return ""; } }
		}

		private void CheckUnusedNodes(XmlNode node)
		{
			unusedNodes.Add(node);
		}
	}
}
