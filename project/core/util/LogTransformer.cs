using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class LogTransformer : ITransformer
	{
		private string _xmlFileName;
		private string _xslFileName;

		public LogTransformer(string logFileName, string xslFilename)
		{
			_xmlFileName = logFileName;
			_xslFileName = xslFilename;
		}

		public string Transform()
		{
			// todo: error handling for:
			// 1. missing file
			// 2. bad xml in log file - raises also point of: should check that our writer has not been duped into writing bad xml (eg. cvs comment has <foo></moo>)
			if (! File.Exists(_xmlFileName))
			{
				throw new CruiseControlException(string.Format("Logfile not found: {0}", _xmlFileName));
			}
			try
			{
				XslTransform transform = new XslTransform();
				LoadStylesheet(transform);
				XmlReader reader = transform.Transform(new XPathDocument(_xmlFileName), null);

				XmlDocument output = new XmlDocument();
				output.Load(reader);
				return output.OuterXml;
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to execute transform: " + _xslFileName, ex);
			}
		}

		private void LoadStylesheet(XslTransform transform)
		{
			try
			{
				transform.Load(_xslFileName);
			}
			catch (FileNotFoundException)
			{
				throw new CruiseControlException(string.Format("XSL stylesheet file not found: {0}", _xslFileName));
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to load transform: " + _xslFileName, ex);
			}
		}

	}
}