using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslTransformer : ITransformer
	{
		public string Transform(string input, string transformerFileName, Hashtable xsltArgs)
		{
			using (StringReader inputReader = new StringReader(input))
			{
				try
				{
					XslTransform transform = new XslTransform();
					LoadStylesheet(transform, transformerFileName);
					StringWriter output = new StringWriter();
					transform.Transform(new XPathDocument(inputReader), CreateXsltArgs(xsltArgs), output);
					return output.ToString();
				}
				catch (XmlException ex)
				{
					throw new CruiseControlException("Unable to execute transform: " + transformerFileName, ex);
				}
			}
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
