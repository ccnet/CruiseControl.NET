using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Test
{
    [TestFixture]
    public class XmlIntegrationResultWriterTest : XmlLogFixture
    {
        public const string TEMP_SUBDIR = "XmlIntegrationResultWriterTest";
        private StringWriter buffer;
        private XmlIntegrationResultWriter _writer;

        [SetUp]
        protected void SetUp()
        {
            buffer = new StringWriter();
            _writer = new XmlIntegrationResultWriter(buffer);
        }

		[Test]
		public void WriteBuildEvent()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);

			_writer.Write(result);

			string expected = string.Format(@"<cruisecontrol project=""proj""><modifications />{0}</cruisecontrol>", CreateExpectedBuildXml(result));
			Assert.AreEqual(expected, buffer.ToString());
		}

        [Test]
        public void WriteModifications()
        {
            Modification[] mods = CreateModifications();
            string expected = string.Format("<modifications>{0}</modifications>", mods[0].ToXml());

            _writer.WriteModifications(mods);
            Assert.AreEqual(expected, buffer.ToString());
        }

        [Test]
        public void WriteExceptionWithEmbeddedCDATA()
        {
            ExceptionTest(new CruiseControlException("message with <xml><![CDATA[<foo/>]]></xml>"), "message with <xml><![CDATA[<foo/>] ]></xml>");
        }

        [Test]
        public void WriteException()
        {
            ExceptionTest(new CruiseControlException("test exception"));
        }

        private void ExceptionTest(Exception exception)
        {
            ExceptionTest(exception, exception.Message);
        }

        private void ExceptionTest(Exception exception, string exceptionMessage)
        {
            IntegrationResult result = IntegrationResultMother.Create(false);
            result.ExceptionResult = exception;

            _writer.Write(result);
            string actual = buffer.ToString();

            Assert.IsTrue(actual.IndexOf(exceptionMessage) > 0);
            Assert.IsTrue(actual.IndexOf(exception.GetType().Name) > 0);

        	XmlUtil.VerifyXmlIsWellFormed(actual);
        }

    	[Test]
        public void WriteExceptionWithEmbeddedXml()
        {
            ExceptionTest(new CruiseControlException("message with <xml><foo/></xml>"));
        }

        [Test]
        public void WriteIntegrationResult()
        {
            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            string output = GenerateBuildOutput(result);
            Assert.AreEqual(CreateExpectedBuildXml(result), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
        }

		[Test]
		public void WriteTaskResultsWithInvalidXmlShouldBeWrappedInCDATA()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.AddTaskResult("<foo>");
			_writer.Write(result);			
			AssertContains("<![CDATA[<foo>]]>", buffer.ToString());
		}

        [Test]
        public void WriteIntegrationResultOutput()
        {
            IntegrationResult result = IntegrationResultMother.CreateSuccessful();
            result.AddTaskResult("<tag></tag>");
            string output = GenerateBuildOutput(result);
            Assert.AreEqual(CreateExpectedBuildXml(result, "<tag></tag>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
        }

        [Test]
        public void WriteIntegrationResultOutputWithEmbeddedCDATA()
        {
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			result.AddTaskResult("<tag><![CDATA[a b <c>]]></tag>");
        	string output = GenerateBuildOutput(result);
        	Assert.AreEqual(CreateExpectedBuildXml(result, "<tag><![CDATA[a b <c>]]></tag>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
        }

		[Test]
		public void WriteIntegrationResultOutputWithMultiLineCDATA()
		{
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			
			StringWriter swWithoutNull = new StringWriter();
			swWithoutNull.WriteLine("<tag><![CDATA[");
			swWithoutNull.WriteLine("This is a line with a null in it");
			swWithoutNull.WriteLine("]]></tag>");
			result.AddTaskResult(swWithoutNull.ToString());

			string expectedResult = CreateExpectedBuildXml(result, swWithoutNull.ToString());
			XmlUtil.VerifyXmlIsWellFormed(expectedResult);
		}

        [Test]
        public void WriteIntegrationResultOutputWithNullCharacterInCDATA()
        {
			IntegrationResult result = IntegrationResultMother.CreateSuccessful();
			StringWriter swWithNull = new StringWriter();
            swWithNull.WriteLine("<tag><![CDATA[");
            swWithNull.WriteLine("This is a line with a null in it\0");
            swWithNull.WriteLine("]]></tag>");
			result.AddTaskResult(swWithNull.ToString());

			string expectedResult = CreateExpectedBuildXml(result, swWithNull.ToString());
			Assert.AreEqual(expectedResult.Replace("\0", string.Empty), GenerateBuildOutput(result));
        }

        [Test]
        public void WriteOutputWithInvalidXml()
        {
            IntegrationResult result = new IntegrationResult();
            result.AddTaskResult("<tag><c></tag>");
            string output = GenerateBuildOutput(result);
        	Assert.AreEqual(CreateExpectedBuildXml(result, @"<![CDATA[<tag><c></tag>]]>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
        }

		[Test]
		public void ShouldStripXmlDeclaration()
		{
			IntegrationResult result = new IntegrationResult();
			result.AddTaskResult(@"<?xml version=""1.0""?> <foo>Data</foo>");
			string output = GenerateBuildOutput(result);
			XmlUtil.VerifyXmlIsWellFormed(output);
			AssertNotContains(output, "<![CDATA");
			AssertNotContains(output, "<?xml");
		}

        [Test]
        public void WriteFailedIntegrationResult()
        {
            IntegrationResult result = new IntegrationResult();
            result.Status = IntegrationStatus.Failure;
            string output = GenerateBuildOutput(result);
            Assert.AreEqual(CreateExpectedBuildXml(result), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
        }

		[Test]
		public void ShouldNotEncloseBuilderOutputInCDATAIfNotSingleRootedXml()
		{
			string nantOut = @"NAnt 0.85 (Build 0.85.1714.0; net-1.0.win32; nightly; 10/09/2004)
Copyright (C) 2001-2004 Gerry Shaw
http://nant.sourceforge.net

<buildresults project=""test"" />";

			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
			result.AddTaskResult(nantOut);

			Assert.AreEqual(CreateExpectedBuildXml(result, nantOut), GenerateBuildOutput(result));
		}

		[Test]
		public void ShouldHandleEmptyLineBeforeXmlDeclaration()
		{
			IntegrationResult result = new IntegrationResult();
			result.AddTaskResult(@"
<?xml version=""1.0""?> <foo>Data</foo>");
			string output = GenerateBuildOutput(result);
			XmlUtil.VerifyXmlIsWellFormed(output);
			AssertNotContains(output, "<![CDATA");
			AssertNotContains(output, "<?xml");
		}

        private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
        {
            IntegrationResult result = IntegrationResultMother.Create(status);
			result.ProjectName = "proj";
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

        private string GenerateBuildOutput(IntegrationResult input)
        {
            _writer.WriteBuildElement(input);
            return buffer.ToString();
        }

        private Modification[] CreateModifications()
        {
            Modification result = new Modification();
            result.Type = "added";
            result.FileName = "ntserver_protocol.dll";
            result.FolderName = "tools";
            result.ModifiedTime = new DateTime(2002, 9, 5, 11, 38, 30);
            result.UserName = "owen";
            result.EmailAddress = "";
            result.Comment = "ccnet self-admin config folder files";

            Modification[] mods = new Modification[1];
            mods[0] = result;
            return mods;
        }
    }
}