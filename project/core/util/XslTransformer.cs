using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslTransformer : ITransformer
	{
		public string Transform(string input, string xslFilename, Hashtable xsltArgs)
		{
			XslTransform transform = NewXslTransform(xslFilename);

			using (StringReader inputReader = new StringReader(input))
			{
				try
				{
					StringWriter output = new StringWriter();
					transform.Transform(new XPathDocument(inputReader), CreateXsltArgs(xsltArgs), output);
					return output.ToString();
				}
				catch (XmlException ex)
				{
					throw new CruiseControlException("Unable to execute transform: " + xslFilename, ex);
				}
			}
		}

		public string TransformToXml(string xslFilename, XPathDocument document)
		{
			XslTransform transform = NewXslTransform(xslFilename);
			try
			{
				StringWriter output = new StringWriter();
				transform.Transform(document, null, new XmlTextWriter(output));
				return output.ToString();
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to execute transform: " + xslFilename, ex);
			}
		}

		private XslTransform NewXslTransform(string transformerFileName)
		{
			XslTransform transform = new XslTransform();
			LoadStylesheet(transform, transformerFileName);
			return transform;
		}

		private XsltArgumentList CreateXsltArgs(Hashtable xsltArgs)
		{
			XsltArgumentList args = new XsltArgumentList();
			if (xsltArgs != null)
			{
				foreach (object key in xsltArgs.Keys)
				{
					args.AddParam(key.ToString(), "", xsltArgs[key]);
				}
			}
			return args;
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
