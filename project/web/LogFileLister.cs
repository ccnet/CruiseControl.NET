using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	/// <summary>
	/// LogFileLister: helper method for code-behind page for Default.aspx.
	/// </summary>
	public class LogFileLister
	{		
		public const string FAILED = "(Failed)";
 
		public static HtmlAnchor[] GetLinks(string path)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(path);

			HtmlAnchor[] links = new HtmlAnchor[filenames.Length];
			for (int i = 0; i < filenames.Length; i++)
			{
				int j = filenames.Length - i - 1;
				links[j] = new HtmlAnchor();
				links[j].Attributes["class"] = GetLinkClass(filenames[i]);
				links[j].HRef = LogFileUtil.CreateUrl(filenames[i]);
				links[j].InnerText = GetDisplayLabel(filenames[i]);
			}
			return links;
		}

		public static void InitAdjacentAnchors(HtmlAnchor previous, HtmlAnchor next, string path, string currentFile)
		{			
			string[] filenames = LogFileUtil.GetLogFileNames(path);
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
					previous.HRef = LogFileUtil.CreateUrl(filenames[previousIndex]);					
					next.HRef = LogFileUtil.CreateUrl(filenames[nextIndex]);					
					return;
				}
			}
			next.HRef = ".";
			previous.HRef = (currentFile == null) ? 
				LogFileUtil.CreateUrl(filenames[filenames.Length-2]) : ".";
		}

		public static string GetDisplayLabel(string logFilename)
		{
			return string.Format("{0} {1}",
				LogFileUtil.GetFormattedDateString(logFilename), 
				GetBuildStatus(logFilename));	
		} 

		public static string GetBuildStatus(string filename)
		{
			if (LogFileUtil.IsSuccessful(filename))
			{
				return string.Format("({0})",LogFileUtil.ParseBuildNumber(filename));
			}
			else 
			{
				return FAILED;
			}
		}

		private static string GetLinkClass(string filename)
		{
			return LogFileUtil.IsSuccessful(filename) ? "link" : "link-failed";
		}

		public static string GetCurrentFilename(DirectoryInfo logDirectory)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(logDirectory.FullName);
			return (filenames.Length == 0) ? null : filenames[filenames.Length - 1];
		}

		public static string Transform(string logfile, string xslfile)
		{
			// todo: error handling for:
			// 1. missing file
			// 2. bad xml in log file - raises also point of: should check that our writer has not been duped into writing bad xml (eg. cvs comment has <foo></moo>)
			if (! File.Exists(logfile))
			{
				throw new CruiseControlException(string.Format("Logfile not found: {0}", logfile));
			}
			try
			{		
				XslTransform transform = new XslTransform();
				LoadStylesheet(transform, xslfile);
				XmlReader reader = transform.Transform(new XPathDocument(logfile), null, new XmlUrlResolver()); 

				XmlDocument output = new XmlDocument();
				output.Load(reader);
				return output.OuterXml;
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException("Unable to execute transform: " + xslfile, ex);
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
				throw new CruiseControlException(string.Format("XSL stylesheet file not found: {0}", xslfile));
			}
			catch(XmlException ex)
			{
				throw new CruiseControlException("Unable to load transform: " + xslfile, ex);
			}
		}
	}
}
