using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace tw.ccnet.core.publishers
{
	/// <summary>
	/// Summary description for BuildLogTransformer.
	/// </summary>
	public class BuildLogTransformer
	{
		public static string Transform(XmlDocument document) 
		{
			StringBuilder builder = new StringBuilder();
			IList list = (IList) ConfigurationSettings.GetConfig("xslFiles");
			foreach (string xslFile in list) 
			{
				builder.Append(Transform(document, xslFile));
			}

			return builder.ToString();
		}

		public static string Transform(XmlDocument document, string xslFile)
		{
			try 
			{		
				XslTransform transform = new XslTransform();
				LoadStylesheet(transform, xslFile);
				XmlReader reader = transform.Transform(document.CreateNavigator(), null, (XmlResolver)null); 
				
				XmlDocument output = new XmlDocument();
				output.Load(reader);
				return output.OuterXml;
			}
			catch(Exception ex) 
			{
				Console.WriteLine(ex);
				throw new CruiseControlException(String.Format("Bad XML in logfile: " + ex.Message));
			}
		}
		
		private static void LoadStylesheet(XslTransform transform, string xslfile) 
		{
			try 
			{
				transform.Load(xslfile);
			}
			catch(FileNotFoundException) 
			{
				throw new CruiseControlException(String.Format("XSL stylesheet file not found: {0}", xslfile));
			}
			catch(XmlException ex) 
			{
				throw new CruiseControlException(String.Format("Bad XML in stylesheet: " + ex.Message));
			}
		}
	}
}
