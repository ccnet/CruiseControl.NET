using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class CompileStylesheetTest
	{
		private readonly string stylesheet = @"xsl\compile.xsl";
		private XslTransform transform;

		[TestFixtureSetUp]
		public void LoadStyleSheet()
		{
			transform = new XslTransform();
			transform.Load(stylesheet);
		}

		[Test]
		public void ShouldRenderErrorMessageAtTheStartOfLine()
		{
			string input = @"error CS1504: Source file 'C:\Beachball\Checkout\shared\Model\beachball-schema1.cs' could not be opened ('The system cannot find the file specified. ')";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertContains("error CS1504: Source file", actualXml);
		}

		[Test]
		public void ShouldNotRenderBuildCompleteMessage()
		{
			string input = @"<![CDATA[ Build complete -- 1 errors, 0 warnings
  ]]>";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertNotContains("Build complete", actualXml);
		}

		[Test]
		public void ShouldNotRenderRulesErrorMessage()
		{
			string input = @"* Rules gave the following errors:";

			string actualXml = LoadStylesheetAndTransformInput(CreateInfoMessage(input));
			CustomAssertion.AssertNotContains("Rules", actualXml);
		}

		private string CreateInfoMessage(string input)
		{
			return string.Format(@"<cruisecontrol><buildresults>
	<message level=""Info"">
{0}
</message></buildresults></cruisecontrol>", input);
		}

		private string LoadStylesheetAndTransformInput(string input)
		{
			XPathDocument document = new XPathDocument(new StringReader(input));
			XmlReader output = transform.Transform(document, null);

			XmlDocument outputDocument = new XmlDocument();
			outputDocument.Load(output);
			return outputDocument.OuterXml;
		}
	}
}