using System;
using NUnit.Framework;
using System.Xml;
using System.IO;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Publishers;

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
            _writer = new XmlIntegrationResultWriter(new XmlTextWriter(buffer));
        }

		[Test]
		public void WriteBuildEvent()
		{
			IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);

			_writer.Write(result);

			string expected = "<cruisecontrol><modifications />" + CreateExpectedBuildXml(result) + "</cruisecontrol>";
			AssertEquals(expected, buffer.ToString());
		}

        [Test]
        public void WriteModifications()
        {
            Modification[] mods = CreateModifications();
            string expected = mods[0].ToXml();

            _writer.Write(mods);
            AssertEquals(expected, buffer.ToString());
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

            Assert(actual.IndexOf(exceptionMessage) > 0);
            Assert(actual.IndexOf(exception.GetType().Name) > 0);

            //verify xml is well-formed
            XmlDocument document = new XmlDocument();
            document.LoadXml(actual);
        }

        [Test]
        public void WriteExceptionWithEmbeddedXml()
        {
            ExceptionTest(new CruiseControlException("message with <xml><foo/></xml>"));
        }

        [Test]
        public void WriteIntegrationResult()
        {
            IntegrationResult result = new IntegrationResult();
            result.Status = IntegrationStatus.Success;
            string output = GenerateBuildOutput(result);
            AssertEquals(CreateExpectedBuildXml(result), output);
        }

        [Test]
        public void WriteIntegrationResultOutput()
        {
            IntegrationResult result = new IntegrationResult();
            result.Status = IntegrationStatus.Success;
            result.Output = "<tag></tag>";
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
            result.Output = "<tag><c></tag>";
            string output = GenerateBuildOutput(result);
            AssertContains("<![CDATA[<tag><c></tag>]]>", output);
        }

        [Test]
        public void WriteOutputWithInvalidXmlContainingCDATACloseCommand()
        {
            IntegrationResult result = new IntegrationResult();
            result.Output = "<tag><c>]]></tag>";
            string output = GenerateBuildOutput(result);
            AssertContains("<![CDATA[<tag><c>] ]></tag>]]>", output);
        }

        [Test]
        public void WriteFailedIntegrationResult()
        {
            IntegrationResult result = new IntegrationResult();
            result.Status = IntegrationStatus.Failure;
            string output = GenerateBuildOutput(result);
            AssertEquals(CreateExpectedBuildXml(result), output);
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