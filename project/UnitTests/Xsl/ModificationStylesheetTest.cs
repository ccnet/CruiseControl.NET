using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	[TestFixture]
	public class ModificationStylesheetTest
	{
		private readonly string stylesheet = @"xsl\modifications.xsl";
		private XslTransform transform;

		[TestFixtureSetUp]
		public void LoadStyleSheet()
		{
			transform = new XslTransform();
			transform.Load(stylesheet);
		}

		[Test]
		public void ShouldOutputDateOfModification()
		{
			string input = @"<cruisecontrol><modifications>
<modification type=""modified"">
	<filename>compile.xsl</filename>
	<project>project/web/xsl</project>
	<date>02 Oct 2002 09:55</date>
	<user>exortech</user>
	<comment>modified stylesheet to view error messages</comment>
</modification></modifications></cruisecontrol>";

			string actualXml = LoadStylesheetAndTransformInput(input);
			CustomAssertion.AssertContains("02 Oct 2002 09:55", actualXml);
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