using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslTransformer : ITransformer
	{
		public string Transform(string input, string transformerFileName)
		{
			using (StringReader inputReader = new StringReader(input))
			{
				try
				{
					XslTransform transform = new XslTransform();
					LoadStylesheet(transform, transformerFileName);
					XmlReader reader = transform.Transform(new XPathDocument(inputReader), null);
					XmlDocument output = new XmlDocument();
					output.Load(reader);
					return output.OuterXml;
				}
				catch (XmlException ex)
				{
					throw new CruiseControlException("Unable to execute transform: " + transformerFileName, ex);
				}
			}
		}

		private void LoadStylesheet(XslTransform transform, string xslFileName)
		{
			try
			{
				transform.Load(xslFileName);
			}
			catch (FileNotFoundException)
			{
				throw new CruiseControlException(string.Format("XSL stylesheet file not found: {0}", xslFileName));
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to load transform: " + xslFileName, ex);
			}
		}
	}
}
