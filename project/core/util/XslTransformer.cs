using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class XslTransformer : ITransformer
	{
        /// <summary>
        /// Transforms the specified input.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="xslFilename">The XSL filename.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string Transform(string input, string xslFilename, Hashtable xsltArgs)
		{
			XslCompiledTransform transform = NewXslTransform(xslFilename);

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

        /// <summary>
        /// Transforms to XML.	
        /// </summary>
        /// <param name="xslFilename">The XSL filename.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string TransformToXml(string xslFilename, XPathDocument document)
		{
            XslCompiledTransform transform = NewXslTransform(xslFilename);
			try
			{
				StringWriter output = new StringWriter();
				XmlTextWriter xmlWriter = new XmlTextWriter(output);
				xmlWriter.Formatting = Formatting.Indented;
				transform.Transform(document, null, xmlWriter);
				return output.ToString();
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to execute transform: " + xslFilename, ex);
			}
		}

        private static XslCompiledTransform NewXslTransform(string transformerFileName)
		{
            XslCompiledTransform transform = new XslCompiledTransform();
			LoadStylesheet(transform, new SystemPath(transformerFileName).ToString());
			return transform;
		}

		private static XsltArgumentList CreateXsltArgs(Hashtable xsltArgs)
		{
			XsltArgumentList args = new XsltArgumentList();
			if (xsltArgs != null)
			{
				foreach (object key in xsltArgs.Keys)
				{
					args.AddParam(key.ToString(),string.Empty, xsltArgs[key]);
				}
			}
			return args;
		}

        private static void LoadStylesheet(XslCompiledTransform transform, string xslFileName)
		{
            XsltSettings settings = new XsltSettings(true, true);

            try
			{
				transform.Load(xslFileName,settings,new XmlUrlResolver());
			}
			catch (FileNotFoundException)
			{
				throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"XSL stylesheet file not found: {0}", xslFileName));
			}
            catch (ArgumentNullException ex)     
			{
                if (Type.GetType ("Mono.Runtime") != null)  // Workaround Mono library issue: https://github.com/mono/mono/issues/20771
				    throw new CruiseControlException("Unable to load transform: " + xslFileName, ex);
                    
                throw;
			}
            catch (XsltException ex)
			{
				throw new CruiseControlException("Unable to load transform: " + xslFileName, ex);
			}
		}
	}
}
