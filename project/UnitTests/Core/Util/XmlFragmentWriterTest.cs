using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class XmlFragmentWriterTest
	{
		private TextWriter baseWriter;
		private XmlFragmentWriter writer;

		[SetUp]
		public void CreateWriter()
		{
			baseWriter = new StringWriter();
			writer = new XmlFragmentWriter(baseWriter);			
		}

		[Test]
		public void ShouldWriteValidXmlContentToUnderlyingWriter()
		{
			writer.WriteNode("<foo><bar /></foo>");
			Assert.AreEqual("<foo><bar /></foo>", baseWriter.ToString());
		}

		[Test]
		public void ShouldWriteInvalidXmlContentToUnderlyingWriterAsCData()
		{
			writer.WriteNode("<foo><bar></foo></bar>");
			Assert.AreEqual("<![CDATA[<foo><bar></foo></bar>]]>", baseWriter.ToString());
		}

		[Test]
		public void ShouldClearBufferIfInvalidXmlContentWrittenTwice()
		{
			writer.WriteNode("<foo><bar></foo></bar>");
			writer.WriteNode("<foo><bar/></foo>");
			Assert.AreEqual("<![CDATA[<foo><bar></foo></bar>]]><foo><bar /></foo>", baseWriter.ToString());
		}

		[Test]
		public void ShouldBeAbleToWriteWhenFragmentIsSurroundedByText()
		{
			writer.WriteNode("outside<foo/>text");
			Assert.AreEqual("outside<foo />text", baseWriter.ToString());			
		}

		[Test]
		public void ShouldBeAbleToWriteWhenFragmentHasMultipleRootElements()
		{
			writer.WriteNode("<foo/><bar/>");
			Assert.AreEqual("<foo /><bar />", baseWriter.ToString());			
		}

		[Test]
		public void ShouldIgnoreXmlDeclaration()
		{
			writer.WriteNode(@"<?xml version=""1.0"" encoding=""utf-16""?><foo/><bar/>");
			Assert.AreEqual("<foo /><bar />", baseWriter.ToString());						
		}

		[Test]
		public void WriteOutputWithInvalidXmlContainingCDATACloseCommand()
		{
			writer.WriteNode("<tag><c>]]></tag>");
			Assert.AreEqual("<![CDATA[<tag><c>] ]></tag>]]>", baseWriter.ToString());
		}

		[Test]
		public void IndentOutputWhenFormattingIsSpecified()
		{
			writer.Formatting = Formatting.Indented;
			writer.WriteNode("<foo><bar/></foo>");
			Assert.AreEqual(@"<foo>
  <bar />
</foo>", baseWriter.ToString());
		}

		[Test]
		public void WriteTextContainingMalformedXmlElements()
		{
			string text = @"log4net:ERROR XmlConfigurator: Failed to find configuration section
'log4net' in the application's .config file. Check your .config file for the

<log4net> and <configSections> elements. The configuration section should
look like: <section name=""log4net""
type=""log4net.Config.Log4NetConfigurationSectionHandler,log4net"" />";
			writer.WriteNode(text);
			Assert.AreEqual(string.Format("<![CDATA[{0}]]>", text), baseWriter.ToString());			
		}

		[Test]
		public void XmlWithoutClosingElementShouldEncloseInCDATA()
		{
			string text = "<a><b><c/></b>";
			writer.WriteNode(text);
			Assert.AreEqual(string.Format("<![CDATA[{0}]]>", text), baseWriter.ToString());			
		}

		[Test]
		public void UnclosedXmlFragmentEndingInCarriageReturnShouldCloseOpenElement()
		{
			string xml = @"<a>
";
			writer.WriteNode(xml);
			Assert.AreEqual(@"<a>
</a>", baseWriter.ToString());
		}

		[Test]
		public void ShouldStripIllegalCharacters()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < 15; i++)
			{
				builder.Append((char)i, 1);				
			}

			writer.WriteNode("<foo>" + builder.ToString() + "</foo>");
			Assert.AreEqual("<foo>\t\n\r</foo>", baseWriter.ToString());
		}
	}
}
