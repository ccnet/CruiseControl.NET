using System;
using System.Globalization;
using System.IO;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	/// <summary>
	/// LogFileLister: helper method for code-behind page for Default.aspx.
	/// </summary>
	public class LogFileLister
	{
		public const string FAILED = "(Failed)";

		private readonly IFormatProvider formatter;

		public LogFileLister() : this(CultureInfo.CurrentCulture)
		{
		}

		public LogFileLister(IFormatProvider formatter)
		{
			this.formatter = formatter;
		}

		public HtmlAnchor[] GetLinks(string path)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(path);

			HtmlAnchor[] links = new HtmlAnchor[filenames.Length];
			for (int i = 0; i < filenames.Length; i++)
			{
				LogFile logFile = new LogFile(filenames[i], formatter);
				int j = filenames.Length - i - 1;
				links[j] = new HtmlAnchor();
				links[j].Attributes["class"] = GetLinkClass(logFile);
				links[j].HRef = LogFileUtil.CreateUrl(filenames[i]);
				links[j].InnerHtml = GetDisplayLabel(logFile);
			}
			return links;
		}

		public void InitAdjacentAnchors(HtmlAnchor previous, HtmlAnchor next, string path, string currentFile)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(path);
			if (filenames.Length <= 1)
			{
				return;
			}

			for (int i = 0; i < filenames.Length; i++)
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
				LogFileUtil.CreateUrl(filenames[filenames.Length - 2]) : ".";
		}

		private string GetDisplayLabel(LogFile logFile)
		{
			return string.Format("<nobr>{0} {1}</nobr>",
			                     logFile.FormattedDateString,
			                     GetBuildStatus(logFile));
		}

		public string GetBuildStatus(LogFile logFile)
		{
			if (logFile.Succeeded)
			{
				return string.Format("({0})", logFile.Label);
			}
			else
			{
				return FAILED;
			}
		}

		private string GetLinkClass(LogFile logFile)
		{
			return logFile.Succeeded ? "link" : "link-failed";
		}

		public string GetCurrentFilename(DirectoryInfo logDirectory)
		{
			string[] filenames = LogFileUtil.GetLogFileNames(logDirectory.FullName);
			return (filenames.Length == 0) ? null : filenames[filenames.Length - 1];
		}

	}
}