using System.IO;
using System.Xml;
using NUnit.Framework;
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
	}
}
