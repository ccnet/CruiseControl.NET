using System;
using System.Configuration;
using System.IO;
using System.Web;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Web
{
	public class WebUtil
	{
		public static string ResolveLogFile(HttpContext context)
		{
			string logfile = WebUtil.GetLogFilename(context, context.Request);
			if (logfile == null)
			{
				throw new CruiseControlException("Internal Error - couldn't resolve logfile to use");
			}
			if (!File.Exists(logfile))
			{
				throw new CruiseControlException(String.Format("Logfile not found: {0}", logfile));
			}
			return logfile;
		}

		public static string GetLogFilename(HttpContext context, HttpRequest request)
		{
			DirectoryInfo logDirectory = GetLogDirectory(context);
			string logfile = request.QueryString[LogFileUtil.LogQueryString];
			if (logfile == null)
			{
				logfile = LogFileLister.GetCurrentFilename(logDirectory);
			}
			return (logfile == null) ? null : Path.Combine(logDirectory.FullName, logfile);
		}

		public static string GetXslFilename(string xslfile, HttpRequest request)
		{
			return Path.Combine(request.MapPath("xsl"), xslfile);
		}

		public static string FormatMultiline(string multilineString)
		{
			return multilineString.Replace(Environment.NewLine, @"<br>");
		}

		private static string LogDir { get { return ConfigurationSettings.AppSettings["logDir"]; } }

		public static DirectoryInfo GetLogDirectory(HttpContext context)
		{
			string dirName = LogDir;
			DirectoryInfo logDirectory = new DirectoryInfo(dirName);
			if (!logDirectory.Exists)
			{
				// Hack to have a guess of whether an absolute path is given in the config
				if (dirName.IndexOf(':') < 0)
				{
					// If so try and treat as relative to the webapp
					logDirectory = new DirectoryInfo(context.Server.MapPath(dirName));
					if (!logDirectory.Exists)
					{
						throw new Exception(string.Format("Can't find log directory [{0}] (Full path : [{1}]", 
							dirName, logDirectory.FullName));
					}
				}
				else
				{
					throw new Exception(string.Format("Can't find log directory [{0}] (Full path : [{1}]", 
						dirName, logDirectory.FullName));
				}
			}
			return logDirectory;
		}
	}
}
