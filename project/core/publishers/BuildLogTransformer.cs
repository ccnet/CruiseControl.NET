using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	/// <summary>
	/// Utility class that provides static methods to transform build results using
	/// Xsl stylesheets.
	/// </summary>
	public class BuildLogTransformer
	{
		/// <summary>
		/// Utility class; not intended for instantiation.
		/// </summary>
		private BuildLogTransformer() {}

		/// <summary>
		/// Transforms the specified Xml document using all configured Xsl files,
		/// and returns the concatenated resulting Xml.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public static string TransformResultsWithAllStyleSheets(XmlDocument document) 
		{
			StringBuilder builder = new StringBuilder();
			IList list = (IList)ConfigurationSettings.GetConfig("xslFiles");
			foreach (string xslFile in list) 
			{
				builder.Append(Transform(document, xslFile));
			}

			return builder.ToString();
		}

		/// <summary>
		/// Transforms an Xml document using a specific Xsl file.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="xslFile"></param>
		/// <returns></returns>
		public static string Transform(XmlDocument document, string xslFile)
		{
			try 
			{		
				XslTransform transform = new XslTransform();
				LoadStylesheet(transform, xslFile);

				XmlReader reader = transform.Transform(document.DocumentElement, null, new XmlUrlResolver()); 
				
				XmlDocument output = new XmlDocument();
				output.Load(reader);
				return output.OuterXml;
			}
			catch (Exception ex) 
			{
				throw new CruiseControlException("Unable to execute transform: " + xslFile, ex);
			}
		}

		/// <summary>
		/// Attempts to load the specified stylesheet.  Throws a <see cref="CruiseControlException"/>
		/// if an error occurs.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="xslfile"></param>
		private static void LoadStylesheet(XslTransform transform, string xslFileName) 
		{
			try 
			{
				transform.Load(xslFileName);
			}
			catch (FileNotFoundException) 
			{
				throw new CruiseControlException("XSL stylesheet file not found: " + xslFileName);
			}
			catch (XmlException ex) 
			{
				throw new CruiseControlException("Bad XML in stylesheet: " + ex.Message);
			}
		}
	}
}
