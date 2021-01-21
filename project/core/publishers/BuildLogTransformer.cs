using System.Collections;
using System.Configuration;
using System.Text;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// Utility class that provides methods to transform build results using
	/// Xsl stylesheets.
	/// </summary>
	public class BuildLogTransformer
	{
		/// <summary>
		/// Transforms the specified Xml document using all configured Xsl files,
		/// and returns the concatenated resulting Xml.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public string TransformResultsWithAllStyleSheets(XPathDocument document)
		{
			var section = ConfigurationManager.GetSection("xslFiles") as XslFilesSectionHandler;
            if (section == null)
                section = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("xslFiles") as XslFilesSectionHandler;
            
			return TransformResults(section?.FileNames, document);
		}

        /// <summary>
        /// Transforms the results.	
        /// </summary>
        /// <param name="xslFiles">The XSL files.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string TransformResults(IList xslFiles, XPathDocument document)
		{
			StringBuilder builder = new StringBuilder();
			if (xslFiles == null)
				return builder.ToString();

			XslTransformer transformer = new XslTransformer();
			foreach (string xslFile in xslFiles)
			{
                Log.Trace("Transforming using file : {0}",xslFile);
				builder.Append(transformer.TransformToXml(xslFile, document));
			}
			return builder.ToString();
		}
	}
}