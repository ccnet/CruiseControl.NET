using System;
using System.IO;
using System.Xml.XPath;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class XmlIntegrationResultWriterTest : XmlLogFixture
	{
		public const string TEMP_SUBDIR = "XmlIntegrationResultWriterTest";
		private StringWriter buffer;
		private XmlIntegrationResultWriter writer;
		private IntegrationResult result;

		[SetUp]
		protected void SetUp()
		{
			buffer = new StringWriter();
			writer = new XmlIntegrationResultWriter(buffer);
			result = IntegrationResultMother.CreateSuccessful();
		}

		[Test]
		public void WriteBuildEvent()
		{
			result = CreateIntegrationResult(IntegrationStatus.Success, false);
			writer.Write(result);
			AssertContains(CreateExpectedBuildXml(result), buffer.ToString());
		}

		[Test]
		public void WriteModifications()
		{
			Modification[] mods = CreateModifications();
			string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<modifications>{0}</modifications>", mods[0].ToXml());

			writer.WriteModifications(mods);
			Assert.AreEqual(expected, buffer.ToString());
		}

		[Test]
		public void WriteRequest()
		{
			writer.Write(result);
			string xml = buffer.ToString();
			AssertXPathExists(xml, string.Format(System.Globalization.CultureInfo.CurrentCulture,"//request[@source='{0}' and @buildCondition='{1}']", 
			                                                   result.IntegrationRequest.Source, result.IntegrationRequest.BuildCondition));

            Assert.AreEqual("Build (IfModificationExists) triggered from foo", result.IntegrationRequest.ToString(), "request section is wrong");
            
            AssertXPathExists(xml, "cruisecontrol/integrationProperties");


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
			result.ExceptionResult = exception;

			writer.Write(result);
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
			string output = GenerateBuildOutput(result);
			Assert.AreEqual(CreateExpectedBuildXml(result), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
		}

		[Test]
		public void WriteTaskResultsWithInvalidXmlShouldBeWrappedInCDATA()
		{
			result.AddTaskResult("<foo>");
			writer.Write(result);
			AssertContains("<![CDATA[<foo>]]>", buffer.ToString());
		}

		[Test]
		public void WriteIntegrationResultOutput()
		{
			result.AddTaskResult("<tag></tag>");
			string output = GenerateBuildOutput(result);
			Assert.AreEqual(CreateExpectedBuildXml(result, "<tag></tag>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
		}

		[Test]
		public void WriteIntegrationResultOutputWithEmbeddedCDATA()
		{
			result.AddTaskResult("<tag><![CDATA[a b <c>]]></tag>");
			string output = GenerateBuildOutput(result);
			Assert.AreEqual(CreateExpectedBuildXml(result, "<tag><![CDATA[a b <c>]]></tag>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
		}

		[Test]
		public void WriteIntegrationResultOutputWithMultiLineCDATA()
		{
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
			StringWriter swWithNull = new StringWriter();
			swWithNull.WriteLine("<tag><![CDATA[");
			swWithNull.WriteLine("This is a line with a null in it\0");
			swWithNull.WriteLine("]]></tag>");
			result.AddTaskResult(swWithNull.ToString());

			string expectedResult = CreateExpectedBuildXml(result, swWithNull.ToString());
			Assert.AreEqual(expectedResult.Replace("\0", string.Empty).Replace("\r", string.Empty), GenerateBuildOutput(result));
		}

		[Test]
		public void WriteOutputWithInvalidXml()
		{
			result.AddTaskResult("<tag><c></tag>");
			string output = GenerateBuildOutput(result);
			Assert.AreEqual(CreateExpectedBuildXml(result, @"<![CDATA[<tag><c></tag>]]>"), output);
			XmlUtil.VerifyXmlIsWellFormed(output);
		}

		[Test]
		public void ShouldStripXmlDeclaration()
		{
			result.AddTaskResult(@"<?xml version=""1.0""?> <foo>Data</foo>");
			string output = GenerateBuildOutput(result);
			XmlUtil.VerifyXmlIsWellFormed(output);
			AssertNotContains(output, "<![CDATA");
			AssertNotContains(output, "<?xml");
		}

		[Test]
		public void WriteFailedIntegrationResult()
		{
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

			result = CreateIntegrationResult(IntegrationStatus.Success, false);
			result.AddTaskResult(nantOut);

			Assert.AreEqual(CreateExpectedBuildXml(result, nantOut), GenerateBuildOutput(result));
		}

		[Test]
		public void ShouldHandleEmptyLineBeforeXmlDeclaration()
		{
			result.AddTaskResult(@"
<?xml version=""1.0""?> <foo>Data</foo>");
			string output = GenerateBuildOutput(result);
			XmlUtil.VerifyXmlIsWellFormed(output);
			AssertNotContains(output, "<![CDATA");
			AssertNotContains(output, "<?xml");
		}

		[Test]
		public void WriteCPlusPlusOutput()
		{
			result.AddTaskResult(@"e:\RW\WORKSPACES\WIN2000\MSVC60\8S\INCLUDE\iterator(563) : warning C4284: return type for 'std::reverse_iterator<class std::vector<bool,class std::allocator>::iterator,bool,class std::vector<bool,class std::allocator>::reference,bool *,int>::operator ->' is 'bool *' (ie; not a UDT or reference to a UDT.  Will produce errors if applied using infix notation)

        e:\RW\WORKSPACES\WIN2000\MSVC60\8S\INCLUDE\vector(1045) : see reference to class template instantiation 'std::reverse_iterator<class std::vector<bool,class std::allocator>::iterator,bool,class std::vector<bool,class std::allocator>::reference,bool *,int>' being compiled

e:\RW\WORKSPACES\WIN2000\MSVC60\8S\INCLUDE\iterator(563) : warning C4284: return type for 'std::reverse_iterator<class std::vector<bool,class std::allocator>::const_iterator,bool,bool,bool const *,int>::operator ->' is 'const bool *' (ie; not a UDT or reference to a UDT.  Will produce errors if applied using infix notation)

        e:\RW\WORKSPACES\WIN2000\MSVC60\8S\INCLUDE\vector(1047) : see reference to class template instantiation 'std::reverse_iterator<class std::vector<bool,class std::allocator>::const_iterator,bool,bool,bool const *,int>' being compiled");
			writer.Write(result);
			new XPathDocument(new StringReader(buffer.ToString()));
		}

		private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
		{
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
			writer.WriteBuildElement(input);
			return buffer.ToString();
		}

		private Modification[] CreateModifications()
		{
			Modification mod = new Modification();
			mod.Type = "added";
			mod.FileName = "ntserver_protocol.dll";
			mod.FolderName = "tools";
			mod.ModifiedTime = new DateTime(2002, 9, 5, 11, 38, 30);
			mod.UserName = "owen";
			mod.EmailAddress =string.Empty;
			mod.Comment = "ccnet self-admin config folder files";

			Modification[] mods = new Modification[1];
			mods[0] = mod;
			return mods;
		}
	}
}