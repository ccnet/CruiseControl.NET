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
	public class XmlLogPublisherTest : CustomAssertion
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
			string xml = string.Format(
				@"		<xmllogger>
		    <logDir>{0}</logDir>
		</xmllogger>", LOGDIR);
			return NetReflector.Read(xml) as XmlLogPublisher;
		}

		public void TestPopulateFromConfig()
		{
			AssertNotNull("Populated publisher is null", _publisher);
			AssertEquals(LOGDIR, _publisher.LogDir);
		}

		public void TestWriteIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			string output = GenerateBuildOutput(result);
			AssertEquals(CreateExpectedBuildXml(result), output);
		}
		
		public void TestWriteIntegrationResultOutput()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Output ="<tag></tag>";
			string output = GenerateBuildOutput(result);
			AssertEquals(CreateExpectedBuildXml(result), output);
		}

		[Test]
		public void WriteIntegrationResultOutputWithEmbeddedCDATA()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			result.Output = "<tag><![CDATA[a b <c>]]></tag>";
			AssertEquals(CreateExpectedBuildXml(result), GenerateBuildOutput(result));
		}

		[Test]
		public void WriteIntegrationResultOutputWithNullCharacterInCDATA()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;

			StringWriter swWithoutNull = new StringWriter();
			swWithoutNull.WriteLine("<tag><![CDATA["); 
			swWithoutNull.WriteLine("This is a line with a null in it");
			swWithoutNull.WriteLine("]]></tag>");
			result.Output = swWithoutNull.ToString();

			string expectedResult = CreateExpectedBuildXml(result);

			StringWriter swWithNull = new StringWriter();
			swWithNull.WriteLine("<tag><![CDATA["); 
			swWithNull.WriteLine("This is a line with a null in it\0");
			swWithNull.WriteLine("]]></tag>");
			result.Output = swWithNull.ToString();

			AssertEquals(expectedResult, GenerateBuildOutput(result));
		}
		
		[Test]
		public void WriteOutputWithInvalidXml()
		{
			IntegrationResult result = new IntegrationResult();
			result.Output ="<tag><c></tag>";
			string output = GenerateBuildOutput(result);
			AssertContains("<![CDATA[<tag><c></tag>]]>", output);
		}

		[Test]
		public void WriteOutputWithInvalidXmlContainingCDATACloseCommand()
		{
			IntegrationResult result = new IntegrationResult();
			result.Output ="<tag><c>]]></tag>";
			string output = GenerateBuildOutput(result);
			AssertContains("<![CDATA[<tag><c>] ]></tag>]]>", output);
		}		

		public void TestWriteFailedIntegrationResult()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Failure;
			string output = GenerateBuildOutput(result);
			AssertEquals(CreateExpectedBuildXml(result), output);
		}
		
		public void TestWriteBuildEvent()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.Write(result, writer);
			string expected = "<cruisecontrol><modifications />" + CreateExpectedBuildXml(result) + "</cruisecontrol>";
			AssertEquals(expected, actual.ToString());
		}

		[Test]
		public void MergeFilesConfig() 
		{
			string xml = string.Format(
				@"		<xmllogger>
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
		public void MergeFile() 
		{
			TempFileUtil.CreateTempDir(TEMP_SUBDIR);
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
			_publisher.MergeFiles = new string [] {LOGDIR + "\\foo.xml"};
			TempFileUtil.CreateTempXmlFile(LOGDIR, "foo.xml", "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"no\"?><foo bar=\"4\">bat</foo>");
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.Write(result, writer);
			string expected = "<cruisecontrol><modifications />" + CreateExpectedBuildXml(result) + "<foo bar=\"4\">bat</foo></cruisecontrol>";
			AssertEquals(expected, actual.ToString());
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
			AssertEquals(LOGDIR + "\\foo.xml", (string)list[0]);

			_publisher.MergeFiles = new string[] {LOGDIR + "\\foo.*"};
			list = _publisher.GetMergeFileList();
			AssertEquals(2, list.Count);
		}
		
		private string CreateExpectedBuildXml(IntegrationResult result)
		{
			string error = (result.Status == IntegrationStatus.Success) ? String.Empty : " error=\"true\"";
			if (result.Output == null)
			{
				return string.Format
					(@"<build date=""{0}"" buildtime=""00:00:00""{1} />", result.StartTime, error);
			}
			else
			{
				return string.Format
					(@"<build date=""{0}"" buildtime=""00:00:00""{1}>{2}</build>", result.StartTime, error, result.Output);
			}
		}

		public void TestGetFilenameForFailedBuild()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Failure, true);
			string expected = "log00010101000000.xml";
			AssertEquals(expected, _publisher.GetFilename(result));
		}
		
		public void TestGetFilenameForGoodBuild()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);
			string expected = "log00010101000000Lbuild.1.xml";
			AssertEquals(expected, _publisher.GetFilename(result));
		}
		
		public void TestPublish()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, true);

			_publisher.PublishIntegrationResults(null, result);

			string filename = _publisher.GetFilename(result);
			string outputPath = Path.Combine(_publisher.LogDir, filename);
			Assert(outputPath + " should exist ", File.Exists(outputPath));
			
			CheckForXml(outputPath);
		}

		public void TestPublish_UnknownIntegrationStatus()
		{
			AssertFalse(LOGDIR + " should be not exist at start of test.", Directory.Exists(LOGDIR));
			_publisher.PublishIntegrationResults(null, new IntegrationResult());
			AssertFalse(LOGDIR + " should still not exist at end of this test.", Directory.Exists(LOGDIR));
		}

		public void TestWriteModifications() 
		{
			Modification[] mods = CreateModifications();
			string expected = mods[0].ToXml();
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.Write(mods, writer);
			AssertEquals(expected, actual.ToString());
		}
		
		[Test]
		public void WriteException()
		{
			ExceptionTest(new CruiseControlException("test exception"));
		}

		[Test]
		public void WriteExceptionWithEmbeddedXml()
		{
			ExceptionTest(new CruiseControlException("message with <xml><foo/></xml>"));
		}

		[Test]
		public void WriteExceptionWithEmbeddedCDATA()
		{
			ExceptionTest(new CruiseControlException("message with <xml><![CDATA[<foo/>]]></xml>"), "message with <xml><![CDATA[<foo/>] ]></xml>");
		}

		private void ExceptionTest(Exception exception)
		{
			ExceptionTest(exception, exception.Message);
		}

		private void ExceptionTest(Exception exception, string exceptionMessage)
		{
			IntegrationResult result = IntegrationResultMother.Create(false);
			result.ExceptionResult = exception;
			StringWriter buffer = new StringWriter();
			_publisher.Write(result, new XmlTextWriter(buffer));
			string actual = buffer.ToString();

			Assert(actual.IndexOf(exceptionMessage) > 0);
			Assert(actual.IndexOf(exception.GetType().Name) > 0);

			//verify xml is well-formed
			XmlDocument document = new XmlDocument();
			document.LoadXml(actual);
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
			AssertNotNull(writer);
			
			writer.WriteStartElement("bar");
			writer.WriteEndElement();
			writer.Close();
			
			Assert(filename + " should exist ",
				TempFileUtil.TempFileExists(TEMP_SUBDIR, filename));
		}

		public void TestGetXmlWriterTwice()
		{
			AssertGetXmlWriter("TestGetXmlWriter1.xml");
			AssertGetXmlWriter("TestGetXmlWriter2.xml");
			Assert("there should be two log files", 
				TempFileUtil.TempFileExists(TEMP_SUBDIR, "TestGetXmlWriter1.xml"));
			AssertEquals(2, Directory.GetFiles(TempFileUtil.GetTempPath(TEMP_SUBDIR)).Length);
		}

		private string GenerateBuildOutput(IntegrationResult input)
		{
			StringWriter actual = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(actual);
			_publisher.WriteBuildElement(input, writer);
			return actual.ToString();
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
