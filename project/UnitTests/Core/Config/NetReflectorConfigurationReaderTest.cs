using System;
using System.Collections;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
{
	[TestFixture]
	public class NetReflectorConfigurationReaderTest
	{
		private NetReflectorConfigurationReader reader;
		private IList invalidNodes;

		[SetUp]
		protected void CreateReader()
		{
			reader = new NetReflectorConfigurationReader();
			reader.InvalidNodeEventHandler += new InvalidNodeEventHandler(CheckInvalidNode);
			invalidNodes = new ArrayList();
		}

		[Test]
		public void DeserialiseSingleProjectFromXml()
		{
			string projectXml = ConfigurationFixture.GenerateProjectXml("test");
			IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml));
			ValidateProject(configuration, "test");
		}

        [Test]
        public void DeserialiseSingleProjectPlusQueueFromXml()
        {
            string projectXml = ConfigurationFixture.GenerateProjectXml("test");
            string queueXml = "<queue name=\"test\" duplicates=\"ApplyForceBuildsReAdd\"/>";
            IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml+queueXml));
            ValidateProject(configuration, "test");
        }

        [Test]
        [ExpectedException(typeof(ConfigurationException), ExpectedMessage="\nUnknown configuration item found\n<garbage />")]
        public void DeserialiseSingleProjectPlusUnknownFromXml()
        {
            string projectXml = ConfigurationFixture.GenerateProjectXml("test");
            string queueXml = "<garbage/>";
            IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml + queueXml));
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
			Assert.AreEqual(1, invalidNodes.Count);
			Assert.AreEqual("bar", ((InvalidNodeEventArgs)invalidNodes[0]).Node.Name);
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

        [Test]
        public void QueueValidationForQueueWithProjectName()
        {
            string projectXml = ConfigurationFixture.GenerateProjectXml("test");
            string queueXml = "<queue name=\"test\" duplicates=\"ApplyForceBuildsReAdd\"/>";
            IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml + queueXml));
            ValidateProject(configuration, "test");
        }

        [Test]
        public void QueueValidationForQueueWithQueueNameInProject()
        {
            string projectXml = ConfigurationFixture.GenerateProjectXml("test", "testQueue");
            string queueXml = "<queue name=\"testQueue\" duplicates=\"ApplyForceBuildsReAdd\"/>";
            IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml + queueXml));
            ValidateProject(configuration, "test");
        }

        [Test]
        [ExpectedException(typeof(ConfigurationException), ExpectedMessage="An unused queue definition has been found: name 'testQueue'")]
        public void QueueValidationForUnreferencedQueue()
        {
            string projectXml = ConfigurationFixture.GenerateProjectXml("test");
            string queueXml = "<queue name=\"testQueue\" duplicates=\"ApplyForceBuildsReAdd\"/>";
            IConfiguration configuration = reader.Read(ConfigurationFixture.GenerateConfig(projectXml + queueXml));
            ValidateProject(configuration, "test");
        }
        
		private void ValidateProject(IConfiguration configuration, string projectName)
		{
			Project project = configuration.Projects[projectName] as Project;
			Assert.AreEqual(projectName, project.Name);
			Assert.IsTrue(project.Tasks[0] is NullTask);
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.AreEqual(1, project.Publishers.Length);
			Assert.IsTrue(project.Publishers[0] is NullTask);
			if (invalidNodes.Count > 0) 
				Assert.Fail("The xml contains nodes that are no longer used: {0}.", ((XmlNode)invalidNodes[0]).OuterXml);				
		}
        [ReflectorType("garbage")]
        class Garbage
        {
        }

		[ReflectorType("customtestproject")]
		class CustomTestProject : ProjectBase, IProject
		{
			public IIntegrationResult Integrate(IntegrationRequest request) { return null; }
			public void NotifyPendingState() {}
			public void NotifySleepingState() {}
			public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment) { }

			public XmlDocument Statistics
			{
				get { throw new NotImplementedException(); }
			}

            public string ModificationHistory
            {
                get { throw new NotImplementedException(); }
            }

            public string RSSFeed
            {
                get { throw new NotImplementedException(); }
            }


			public IIntegrationRepository IntegrationRepository
			{
				get { throw new NotImplementedException(); }
			}

			public string QueueName
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public int QueuePriority
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}

			public void Initialize() {}

			public ProjectStatus CreateProjectStatus(IProjectIntegrator integrator)
			{
				throw new NotImplementedException();
			}

            public ProjectActivity CurrentActivity
            {
                get { throw new NotImplementedException(); }
            }
			
			public void AbortRunningBuild()
			{
				throw new NotImplementedException();
			}
			
			public void AddMessage(Message message)
			{
				throw new NotImplementedException();
			}

			public string WebURL { get {return ""; } }
		}

		private void CheckInvalidNode(InvalidNodeEventArgs args)
		{
			invalidNodes.Add(args);
		}
	}
}
