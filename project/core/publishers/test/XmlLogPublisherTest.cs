using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
    [TestFixture]
    public class XmlLogPublisherTest : XmlLogFixture
    {
        public static readonly string TEMP_SUBDIR = "XmlLogPublisherTest";
        public static readonly string LOGDIR = TempFileUtil.GetTempPath(TEMP_SUBDIR);

        private XmlLogPublisher _publisher;
    	private string artifactsDirPath;

    	[SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(TEMP_SUBDIR);
            _publisher = CreatePublisher();
			artifactsDirPath = TempFileUtil.CreateTempDir("artifacts");
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(TEMP_SUBDIR);
			TempFileUtil.DeleteTempDir("artifacts");
		}

        private XmlLogPublisher CreatePublisher()
        {
            string xml = string.Format(@"		<xmllogger>
		    <logDir>{0}</logDir>
		</xmllogger>", LOGDIR);
            return NetReflector.Read(xml) as XmlLogPublisher;
        }

        [Test]
        public void PopulateFromConfig()
        {
            Assert.IsNotNull(_publisher);
            Assert.AreEqual(LOGDIR, _publisher.ConfiguredLogDirectory);
        }

        [Test]
        public void MergeFilesConfig()
        {
            string xml = string.Format(@"		<xmllogger>
		    <logDir>{0}</logDir>
			<mergeFiles>
				<file>d:\foo.xml</file>
			</mergeFiles>
		</xmllogger>", LOGDIR);
            XmlLogPublisher pub = NetReflector.Read(xml) as XmlLogPublisher;
            Assert.AreEqual(1, pub.MergeFiles.Length);
            Assert.AreEqual(@"d:\foo.xml", pub.MergeFiles[0]);
        }

        [Test]
        public void GetFilenameForFailedBuild()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Failure,  true);
            string expected = "log19800101000000.xml";
            Assert.AreEqual(expected, _publisher.GetFilename(result));
        }

        [Test]
        public void GetFilenameForGoodBuild()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);
            string expected = "log19800101000000Lbuild.1.xml";
            Assert.AreEqual(expected, _publisher.GetFilename(result));
        }

        [Test]
        public void Publish()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);

            _publisher.PublishIntegrationResults(null, result);

            string filename = _publisher.GetFilename(result);
            string outputPath = Path.Combine(_publisher.ConfiguredLogDirectory, filename);
            Assert.IsTrue(File.Exists(outputPath), outputPath + " should exist ");

            CheckForXml(outputPath);
        }

		[Test]
		public void ShouldUseProjectArtifactDirectoryInLogDirectoryIfLogDirNotSetWhenPublishing()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);

			DynamicMock projectMock = new DynamicMock(typeof(IProject));
			projectMock.ExpectAndReturn("ArtifactDirectory", artifactsDirPath);

			_publisher = new XmlLogPublisher();
			_publisher.PublishIntegrationResults((IProject) projectMock.MockInstance, result);

			string filename = _publisher.GetFilename(result);
			string outputPath = Path.Combine(Path.Combine(artifactsDirPath, "buildlogs"), filename);
			Assert.IsTrue(File.Exists(outputPath), outputPath + " should exist ");

			CheckForXml(outputPath);
			projectMock.Verify();
		}

		[Test]
		public void ShouldGiveLogDirectoryAsConfiguredOneIfOneIsConfigured()
		{
			Assert.AreEqual(LOGDIR, _publisher.LogDirectory(new Project()));
		}

		[Test]
		public void ShouldUseProjectArtifactDirectoryInLogDirectoryIfLogDirNotSet()
		{
			DynamicMock projectMock = new DynamicMock(typeof(IProject));
			projectMock.ExpectAndReturn("ArtifactDirectory", artifactsDirPath);
			_publisher = new XmlLogPublisher();
			Assert.AreEqual(Path.Combine(artifactsDirPath, "buildlogs"), _publisher.LogDirectory((IProject) projectMock.MockInstance));
			projectMock.Verify();
		}

		[Test]
        public void Publish_UnknownIntegrationStatus()
        {
            AssertFalse(LOGDIR + " should be not exist at start of test.", Directory.Exists(LOGDIR));
            _publisher.PublishIntegrationResults(null, new IntegrationResult());
            AssertFalse(LOGDIR + " should still not exist at end of this test.", Directory.Exists(LOGDIR));
        }

        [Test]
        public void MergeFile()
        {
            TempFileUtil.CreateTempDir(TEMP_SUBDIR);
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
            string logDir = TempFileUtil.GetTempPath(TEMP_SUBDIR);
			string logFile = TempFileUtil.CreateTempXmlFile(logDir, "foo.xml", "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"no\"?><foo bar=\"4\">bat</foo>");
			TempFileUtil.CreateTempXmlFile(logDir, "zip.xml", "<zip/>");

            _publisher.ConfiguredLogDirectory = logDir;
            _publisher.MergeFiles = new string [] {logFile, Path.Combine(logDir, "zi*.xml")};		// include wildcard filename
            _publisher.PublishIntegrationResults(null, result);

            string expected = XmlUtil.GenerateIndentedOuterXml("<cruisecontrol project=\"proj\"><modifications />" + CreateExpectedBuildXml(result) + "<foo bar=\"4\">bat</foo><zip /></cruisecontrol>");
            string actualFilename = Path.Combine(logDir, _publisher.GetFilename(result));
            using (StreamReader textReader = File.OpenText(actualFilename))
            {
				Assert.AreEqual(expected, textReader.ReadToEnd());    
            }
        }

        private void CheckForXml(string path)
        {
        	XmlDocument doc = new XmlDocument();
            doc.Load(path);
        }

        [Test]
        public void GetXmlWriter()
        {
            AssertGetXmlWriter("TestGetXmlWriter.xml");
        }

        private void AssertGetXmlWriter(string filename)
        {
            XmlWriter writer = _publisher.GetXmlWriter(TempFileUtil.GetTempPath(TEMP_SUBDIR), filename);
            Assert.IsNotNull(writer);

            writer.WriteStartElement("bar");
            writer.WriteEndElement();
            writer.Close();

            Assert.IsTrue(TempFileUtil.TempFileExists(TEMP_SUBDIR, filename));
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

        [Test]
        public void GetXmlWriterTwice()
        {
            AssertGetXmlWriter("TestGetXmlWriter1.xml");
            AssertGetXmlWriter("TestGetXmlWriter2.xml");
            Assert.IsTrue(TempFileUtil.TempFileExists(TEMP_SUBDIR, "TestGetXmlWriter1.xml"), "there should be two log files");
            Assert.AreEqual(2, Directory.GetFiles(TempFileUtil.GetTempPath(TEMP_SUBDIR)).Length);
        }


    }
}