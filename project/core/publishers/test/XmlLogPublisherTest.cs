using System;
using System.Collections;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
    [TestFixture]
    public class XmlLogPublisherTest : XmlLogFixture
    {
        public const string TEMP_SUBDIR = "XmlLogPublisherTest";
        public static readonly string LOGDIR = TempFileUtil.GetTempPath(TEMP_SUBDIR);

        private XmlLogPublisher _publisher;

        [SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(TEMP_SUBDIR);
            _publisher = CreatePublisher();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(TEMP_SUBDIR);
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
            AssertNotNull("Populated publisher is null", _publisher);
            AssertEquals(LOGDIR, _publisher.LogDir);
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
            AssertEquals(1, pub.MergeFiles.Length);
            AssertEquals(@"d:\foo.xml", pub.MergeFiles[0]);
        }

        [Test]
        public void ResolveWildCards()
        {
            TempFileUtil.CreateTempDir(TEMP_SUBDIR);
            TempFileUtil.CreateTempDir(TEMP_SUBDIR + "\\sub");
            TempFileUtil.CreateTempXmlFile(LOGDIR, "foo.xml", "<foo bar=\"4\">bat</foo>");
            TempFileUtil.CreateTempFile(LOGDIR, "foo.bat", "blah");
            TempFileUtil.CreateTempXmlFile(LOGDIR + "\\sub", "foo.xml", "<foo bar=\"9\">bat</foo>");

            _publisher.MergeFiles = new string[] {LOGDIR + "\\*.xml"};
            ArrayList list = _publisher.GetMergeFileList();
            AssertEquals(1, list.Count);
            AssertEquals(LOGDIR + "\\foo.xml", ((FileInfo) list[0]).FullName);

            _publisher.MergeFiles = new string[] {LOGDIR + "\\foo.*"};
            list = _publisher.GetMergeFileList();
            AssertEquals(2, list.Count);
        }

        [Test]
        public void GetFilenameForFailedBuild()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Failure, true);
            string expected = "log19800101000000.xml";
            AssertEquals(expected, _publisher.GetFilename(result));
        }

        [Test]
        public void GetFilenameForGoodBuild()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);
            string expected = "log19800101000000Lbuild.1.xml";
            AssertEquals(expected, _publisher.GetFilename(result));
        }

        [Test]
        public void Publish()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);

            _publisher.PublishIntegrationResults(null, result);

            string filename = _publisher.GetFilename(result);
            string outputPath = Path.Combine(_publisher.LogDir, filename);
            Assert(outputPath + " should exist ", File.Exists(outputPath));

            CheckForXml(outputPath);
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

            _publisher.LogDir = logDir;
            _publisher.MergeFiles = new string [] {logFile, Path.Combine(logDir, "zi*.xml")};		// include wildcard filename
            _publisher.PublishIntegrationResults(null, result);

            string expected = @"<cruisecontrol project=""proj""><modifications />" + CreateExpectedBuildXml(result) + "<foo bar=\"4\">bat</foo><zip /></cruisecontrol>";
            string actualFilename = Path.Combine(logDir, _publisher.GetFilename(result));
            using (StreamReader textReader = File.OpenText(actualFilename))
            {
				AssertEquals(expected, textReader.ReadToEnd());    
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
            AssertNotNull(writer);

            writer.WriteStartElement("bar");
            writer.WriteEndElement();
            writer.Close();

            Assert(filename + " should exist ", TempFileUtil.TempFileExists(TEMP_SUBDIR, filename));
        }

        private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
        {
            IntegrationResult result = new IntegrationResult("proj");
            result.Label = "1";
            result.Status = status;
            if (addModifications)
            {
                result.Modifications = new Modification[1];
                result.Modifications[0] = new Modification();
                result.Modifications[0].ModifiedTime = new DateTime(2002, 2, 3);
            }
            return result;
        }

        [Test]
        public void GetXmlWriterTwice()
        {
            AssertGetXmlWriter("TestGetXmlWriter1.xml");
            AssertGetXmlWriter("TestGetXmlWriter2.xml");
            Assert("there should be two log files", TempFileUtil.TempFileExists(TEMP_SUBDIR, "TestGetXmlWriter1.xml"));
            AssertEquals(2, Directory.GetFiles(TempFileUtil.GetTempPath(TEMP_SUBDIR)).Length);
        }


    }
}