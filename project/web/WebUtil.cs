using System;
using System.Configuration;
using System.IO;
using System.Web;

using tw.ccnet.core;

namespace tw.ccnet.web
{
	public class WebUtil
	{
		public static string GetLogFilename(HttpContext Context, HttpRequest request)
		{
			DirectoryInfo logDirectory = GetLogDirectory(Context);
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

		public static string FormatException(Exception ex)
		{
			string message = ex.Message.Replace(Environment.NewLine, "<br>");
			return "<br/>ERROR: " + message;
		}

		public static DirectoryInfo GetLogDirectory(HttpContext Context)
		{
			string dirName = ConfigurationSettings.AppSettings["logDir"];
			DirectoryInfo logDirectory = new DirectoryInfo(dirName);
			if (!logDirectory.Exists)
			{
				// Hack to have a guess of whether an absolute path is given in the config
				if (dirName.IndexOf(':') < 0)
				{
					// If so try and treat as relative to the webapp
					logDirectory = new DirectoryInfo(Context.Server.MapPath(dirName));
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
