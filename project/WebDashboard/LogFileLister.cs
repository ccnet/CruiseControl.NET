using System;
using System.IO;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	/// <summary>
	/// LogFileLister: helper method for code-behind page for Default.aspx.
	/// </summary>
	public class LogFileLister
	{		
		public const string FAILED = "(Failed)";
 
		public static HtmlAnchor[] GetLinks(string path, string projectName)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(path);

			HtmlAnchor[] links = new HtmlAnchor[filenames.Length];
			for (int i = 0; i < filenames.Length; i++)
			{
				int j = filenames.Length - i - 1;
				links[j] = new HtmlAnchor();
				links[j].Attributes["class"] = GetLinkClass(filenames[i]);
				links[j].HRef = LogFileUtil.CreateUrl(filenames[i], projectName);
				links[j].InnerHtml = GetDisplayLabel(filenames[i]);
			}
			return links;
		}

		public static void InitAdjacentAnchors(HtmlAnchor latest, HtmlAnchor previous, HtmlAnchor next, string path, string currentFile, string projectName)
		{			
			string[] filenames = LogFileUtil.GetLogFileNames(path);
			if (filenames.Length <= 1)
			{
				return;
			}

			// Yuck - repeated code (and its wrong - can't we get the current URL?)
			latest.HRef = "projectreport.aspx" + string.Format("?{0}={1}", LogFileUtil.ProjectQueryString, projectName);

			for (int i=0; i < filenames.Length; i++)
			{
				if (filenames[i] == currentFile)
				{					
					int previousIndex = Math.Max(0, i - 1);
					int nextIndex = Math.Min(filenames.Length - 1, i + 1);
					previous.HRef = LogFileUtil.CreateUrl(filenames[previousIndex],projectName);					
					next.HRef = LogFileUtil.CreateUrl(filenames[nextIndex],projectName);					
					return;
				}
			}
			// Yuck - repeated code
			next.HRef = "." + string.Format("?{0}={1}", LogFileUtil.ProjectQueryString, projectName);
			previous.HRef = (currentFile == null) ? 
				LogFileUtil.CreateUrl(filenames[filenames.Length-2],projectName) : ".";
		}

		public static string GetDisplayLabel(string logFilename)
		{
			return string.Format("<nobr>{0} {1}</nobr>",
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

	}
}
