using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
    [TestFixture]
    public class XmlLogPublisherTest : XmlLogFixture
    {
        public static readonly string FULL_CONFIGURED_LOG_DIR = "FullConfiguredLogDir";
        public static readonly string FULL_CONFIGURED_LOG_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(FULL_CONFIGURED_LOG_DIR));

		public static readonly string ARTIFACTS_DIR = "Artifacts";
		public static readonly string ARTIFACTS_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(ARTIFACTS_DIR));

        private XmlLogPublisher _publisher;

    	[SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
			TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
			TempFileUtil.CreateTempDir(ARTIFACTS_DIR);

			_publisher = new XmlLogPublisher();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
			TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
		}

        [Test]
        public void PopulateFromConfig()
        {
			string xml = string.Format(@"<xmllogger><logDir>foo</logDir></xmllogger>", FULL_CONFIGURED_LOG_DIR_PATH);
			_publisher = NetReflector.Read(xml) as XmlLogPublisher;
			Assert.IsNotNull(_publisher);
            Assert.AreEqual("foo", _publisher.ConfiguredLogDirectory);
        }

		[Test]
		public void ShouldPublishSuccessfulBuildWithRelativeConfiguredPath()
		{
			// Setup
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success,  true);
			result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
			_publisher.ConfiguredLogDirectory = "relativePath";

			// Execute
			_publisher.PublishIntegrationResults(result);

			// Verify
			string expectedOutputPath = Path.Combine(Path.Combine(ARTIFACTS_DIR_PATH, "relativePath"), "log19800101000000Lbuild.1.xml");
			Assert.IsTrue(File.Exists(expectedOutputPath));
			CheckForXml(expectedOutputPath);
		}

		[Test]
		public void ShouldPublishSuccessfulBuildWithNoConfiguredPath()
		{
			// Setup
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success,  true);
			result.ArtifactDirectory = ARTIFACTS_DIR_PATH;

			// Execute
			_publisher.PublishIntegrationResults(result);

			// Verify
			string expectedOutputPath = Path.Combine(Path.Combine(ARTIFACTS_DIR_PATH, XmlLogPublisher.DEFAULT_LOG_SUBDIRECTORY), "log19800101000000Lbuild.1.xml");
			Assert.IsTrue(File.Exists(expectedOutputPath));
			CheckForXml(expectedOutputPath);
		}

		[Test]
        public void ShouldPublishFailedBuildWithFullConfiguredPath()
        {
			// Setup
			_publisher.ConfiguredLogDirectory = FULL_CONFIGURED_LOG_DIR_PATH;
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Failure,  true);

			// Execute
			_publisher.PublishIntegrationResults(result);

			// Verify
			string expectedOutputPath = Path.Combine(FULL_CONFIGURED_LOG_DIR_PATH, "log19800101000000.xml");
			Assert.IsTrue(File.Exists(expectedOutputPath));
			CheckForXml(expectedOutputPath);
		}

        [Test]
        public void ShouldPublishSuccessfulBuildWithFullConfiguredPath()
        {
			// Setup
			_publisher.ConfiguredLogDirectory = FULL_CONFIGURED_LOG_DIR_PATH;
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success,  true);

			// Execute
			_publisher.PublishIntegrationResults(result);

			// Verify
			string expectedOutputPath = Path.Combine(FULL_CONFIGURED_LOG_DIR_PATH, "log19800101000000Lbuild.1.xml");
			Assert.IsTrue(File.Exists(expectedOutputPath));
			CheckForXml(expectedOutputPath);
        }

		[Test]
        public void ShouldNotPublishResultsWithUnknownStatus()
        {
            AssertFalse(FULL_CONFIGURED_LOG_DIR_PATH + " should not exist at start of test.", Directory.Exists(FULL_CONFIGURED_LOG_DIR_PATH));
            _publisher.PublishIntegrationResults(new IntegrationResult());
            AssertFalse(FULL_CONFIGURED_LOG_DIR_PATH + " should still not exist at end of this test.", Directory.Exists(FULL_CONFIGURED_LOG_DIR_PATH));
        }

        private void CheckForXml(string path)
        {
        	XmlDocument doc = new XmlDocument();
            doc.Load(path);
        }

        private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
        {
        	IntegrationResult result = IntegrationResultMother.Create(status, new DateTime(1980, 1, 1));
			result.ProjectName = "proj";
			result.StartTime = new DateTime(1980, 1, 1);
            result.Label = "1";
            result.Status = status;
            if (addModifications)
            {
            	Modification[] modifications = new Modification[1];
                modifications[0] = new Modification();
                modifications[0].ModifiedTime = new DateTime(2002, 2, 3);
            	result.Modifications = modifications;
            }
            return result;
        }
    }
}