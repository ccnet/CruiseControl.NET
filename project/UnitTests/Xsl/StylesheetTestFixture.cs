using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;

namespace ThoughtWorks.CruiseControl.UnitTests.Xsl
{
	public abstract class StylesheetTestFixture
	{
		private XslTransform transform;

		protected abstract string Stylesheet { get; }

		protected string LoadStylesheetAndTransformInput(string input)
		{
			XPathDocument document = new XPathDocument(new StringReader(input));
			XmlReader output = transform.Transform(document, null);

			XmlDocument outputDocument = new XmlDocument();
			outputDocument.Load(output);
			return outputDocument.OuterXml;
		}

		[TestFixtureSetUp]
		public void LoadStyleSheet()
		{
			transform = new XslTransform();
			transform.Load(Stylesheet);
		}
	}
}
