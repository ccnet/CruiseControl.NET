using System;
using System.IO;
using System.Web;
using tw.ccnet.core;

namespace tw.ccnet.web
{
	public class WebUtil
	{
		public static string GetLogFilename(string path, HttpRequest request)
		{
			string logfile = request.QueryString[LogFile.LogQueryString];
			if (logfile == null)
			{
				logfile = LogFileLister.GetCurrentFilename(path);
			}
			return (logfile == null) ? null : Path.Combine(path, logfile);
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
	}
}
