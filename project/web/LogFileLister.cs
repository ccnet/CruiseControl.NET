using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;
using tw.ccnet.core;

namespace tw.ccnet.web
{
	/// <summary>
	/// LogFileLister: helper method for code-behind page for Default.aspx.
	/// </summary>
	public class LogFileLister
	{		
		public const string FAILED = "(Failed)";
 
		public static HtmlAnchor[] GetLinks(string path)
		{
			string[] filenames = LogFile.GetLogFileNames(path);

			HtmlAnchor[] links = new HtmlAnchor[filenames.Length];
			for (int i = 0; i < filenames.Length; i++)
			{
				int j = filenames.Length - i - 1;
				links[j] = new HtmlAnchor();
				links[j].Attributes["class"] = GetLinkClass(filenames[i]);
				links[j].HRef = LogFile.CreateUrl(filenames[i]);
				links[j].InnerText = GetDisplayLabel(filenames[i]);
			}
			return links;
		}

		public static void InitAdjacentAnchors(HtmlAnchor previous, HtmlAnchor next, string path, string currentFile)
		{			
			string[] filenames = LogFile.GetLogFileNames(path);
			if (filenames.Length <= 1)
			{
				return;
			}

			for (int i=0; i < filenames.Length; i++)
			{
				if (filenames[i] == currentFile)
				{					
					int previousIndex = Math.Max(0, i - 1);
					int nextIndex = Math.Min(filenames.Length - 1, i + 1);
					previous.HRef = LogFile.CreateUrl(filenames[previousIndex]);					
					next.HRef = LogFile.CreateUrl(filenames[nextIndex]);					
					return;
				}
			}
			next.HRef = ".";
			previous.HRef = (currentFile == null) ? 
				LogFile.CreateUrl(filenames[filenames.Length-2]) : ".";
		}

		public static string GetDisplayLabel(string logFilename)
		{
			return String.Format("{0} {1}",
				LogFile.GetFormattedDateString(logFilename), 
				GetBuildStatus(logFilename));	
		} 

		public static string GetBuildStatus(string filename)
		{
			if (LogFile.IsSuccessful(filename))
			{
				return String.Format("({0})",LogFile.ParseBuildNumber(filename));
			}
			else 
			{
				return FAILED;
			}
		}

		private static string GetLinkClass(string filename)
		{
			return LogFile.IsSuccessful(filename) ? "link" : "link-failed";
		}

		public static string GetCurrentFilename(string path)
		{
			string[] filenames = LogFile.GetLogFileNames(path);
			return (filenames.Length == 0) ? null : filenames[filenames.Length - 1];
		}

		public static string Transform(string logfile, string xslfile)
		{
			// todo: error handling for:
			// 1. missing file
			// 2. bad xml in log file - raises also point of: should check that our writer has not been duped into writing bad xml (eg. cvs comment has <foo></moo>)
			if (! File.Exists(logfile))
			{
				throw new CruiseControlException(String.Format("Logfile not found: {0}", logfile));
			}
			try
			{		
				XmlDocument document = new XmlDocument();
				document.Load(logfile);

				XslTransform transform = new XslTransform();
				LoadStylesheet(transform, xslfile);
				XmlReader reader = transform.Transform(document.CreateNavigator(), null, (XmlResolver)null); 

				XmlDocument output = new XmlDocument();
				output.Load(reader);
				return output.OuterXml;
			}
			catch(XmlException ex)
			{
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
