using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Exortech.NetReflector;
using tw.ccnet.core.util;
using tw.ccnet.core.test;
using tw.ccnet.remote;

namespace tw.ccnet.core.publishers.test 
{
	[TestFixture]
	public class XmlLogPublisherTest 
	{
		public const string TEMP_SUBDIR="XmlLogPublisherTest";
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
			string xml = String.Format(
@"		<xmllogger>
		    <logDir>{0}</logDir>
		</xmllogger>", LOGDIR);
			XmlNode node = XmlUtil.CreateDocumentElement(xml);
			XmlPopulator populator = new XmlPopulator();
			return (XmlLogPublisher)populator.Populate(node);
		}

		public void TestPopulateFromConfig()
		{
			Assertion.AssertNotNull("Populated publisher is null", _publisher);
			Assertion.AssertEquals(LOGDIR, _publisher.LogDir);
		}

		public void TestWriteIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			CheckXml(CreateExpectedBuildXml(result), result);
		}
		
		public void TestWriteIntegrationResultOutput()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Output ="<tag></tag>";
			CheckXml(CreateExpectedBuildXml(result), result);
		}
		
		public void TestWriteFailedIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Failure;
			CheckXml(CreateExpectedBuildXml(result), result);
		}
		
		public void TestWriteBuildEvent()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.Write(result, writer);
			string expected = "<cruisecontrol><modifications />" + CreateExpectedBuildXml(result) + "</cruisecontrol>";
			Assertion.AssertEquals(expected, actual.ToString());
		}
		
		private string CreateExpectedBuildXml(IntegrationResult result)
		{
			string error = (result.Status == IntegrationStatus.Success) ? String.Empty : " error=\"true\"";
			if (result.Output == null)
			{
				return String.Format
					(@"<build date=""{0}"" buildtime=""00:00:00""{1} />", result.StartTime, error);
			}
			else
			{
				return String.Format
					(@"<build date=""{0}"" buildtime=""00:00:00""{1}>{2}</build>", result.StartTime, error, result.Output);
			}
		}

		public void TestGetFilenameForFailedBuild()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Failure, true);
			string expected = "log20020203000000.xml";
			Assertion.AssertEquals(expected, _publisher.GetFilename(result));
		}
		
		public void TestGetFilenameForGoodBuild()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);
			string expected = "log20020203000000Lbuild.1.xml";
			Assertion.AssertEquals(expected, _publisher.GetFilename(result));
		}
		
		public void TestPublish()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);

			_publisher.Publish(null, result);

			string filename = _publisher.GetFilename(result);
			string outputPath = Path.Combine(_publisher.LogDir, filename);
			Assertion.Assert(outputPath + " should exist ", File.Exists(outputPath));
			
			CheckForXml(outputPath);
		}

		public void TestWriteModifications() 
		{
			Modification[] mods = CreateModifications();
			string expected = mods[0].ToXml();
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.Write(mods, writer);
			Assertion.AssertEquals(expected, actual.ToString());
		}
		
		private Modification[] CreateModifications()
		{
			Modification result = new Modification();
			result.Type="added";
			result.FileName="ntserver_protocol.dll";
			result.FolderName="tools";
			result.ModifiedTime=new DateTime(2002,9,5,11,38,30);
			result.UserName = "owen";
			result.EmailAddress="";
			result.Comment="ccnet self-admin config folder files";
			
			Modification[] mods = new Modification[1];
			mods[0] = result;
			return mods;
		}
		
		private void CheckForXml(string path)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
		}
		
		public void TestGetXmlWriter()
		{
			AssertGetXmlWriter("TestGetXmlWriter.xml");
		}

		private void AssertGetXmlWriter(string filename)
		{
			XmlWriter writer = _publisher.GetXmlWriter(
				TempFileUtil.GetTempPath(TEMP_SUBDIR), filename);
			Assertion.AssertNotNull(writer);
			
			writer.WriteStartElement("bar");
			writer.WriteEndElement();
			writer.Close();
			
			Assertion.Assert(filename + " should exist ",
				TempFileUtil.TempFileExists(TEMP_SUBDIR, filename));
		}

		public void TestGetXmlWriterTwice()
		{
			AssertGetXmlWriter("TestGetXmlWriter1.xml");
			AssertGetXmlWriter("TestGetXmlWriter2.xml");
			Assertion.Assert("there should be two log files", 
				TempFileUtil.TempFileExists(TEMP_SUBDIR, "TestGetXmlWriter1.xml"));
			Assertion.AssertEquals(2, Directory.GetFiles(TempFileUtil.GetTempPath(TEMP_SUBDIR)).Length);
		}

		private void CheckXml(string expected, IntegrationResult input)
		{
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.WriteInfo(input, writer);
			Assertion.AssertEquals(expected, actual.ToString());
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
		{
			IntegrationResult result = new IntegrationResult();
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
	}
} 
