using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class WebUtil
	{
		private readonly IConfigurationGetter configurationGetter;
		private readonly NameValueCollection queryString;
		private readonly IPathMapper pathMapper;

		// ToDo - get rid of this when we have enough CDI happening
		public static WebUtil Create(HttpRequest request, HttpContext context)
		{
			return new WebUtil(new ConfigurationSettingsConfigGetter(), request.QueryString, new HttpContextPathMapper(context));
		}

		public WebUtil(IConfigurationGetter configurationGetter, NameValueCollection queryString, IPathMapper pathMapper)
		{
			this.configurationGetter = configurationGetter;
			this.queryString = queryString;
			this.pathMapper = pathMapper;
		}

		public string GetLogFileAndCheckItExists()
		{
			string logfile = GetLogFilename();
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

		public DirectoryInfo GetLogDirectory()
		{
			string dirName = GetLogDirName();
			DirectoryInfo logDirectory = new DirectoryInfo(dirName);
			if (!logDirectory.Exists)
			{
				// Hack to have a guess of whether an absolute path is given in the config
				if (dirName.IndexOf(':') < 0)
				{
					// If so try and treat as relative to the webapp
					logDirectory = new DirectoryInfo(pathMapper.MapPath(dirName));
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

		public string GetXslFilename(string xslfile)
		{
			return Path.Combine(pathMapper.MapPath("xsl"), xslfile);
		}

		public string GetCurrentlyViewedProjectName()
		{
			return queryString[LogFileUtil.ProjectQueryString];
		}

		private string GetLogFilename()
		{
			DirectoryInfo logDirectory = GetLogDirectory();
			string logfile = queryString[LogFileUtil.LogQueryString];
			if (logfile == null)
			{
				logfile = LogFileLister.GetCurrentFilename(logDirectory);
			}
			return (logfile == null) ? null : Path.Combine(logDirectory.FullName, logfile);
		}

		private string GetLogDirName()
		{
			object projects = configurationGetter.GetConfig("CCNet/projects");
			if (projects == null)
			{
				throw new ApplicationException("<projects> section not configured correctly in web.config");
			}

			string requestedProject = GetCurrentlyViewedProjectName();
			if (requestedProject == null || requestedProject == string.Empty)
			{
				throw new ApplicationException("[project] parameter not specified on query string");
			}

			foreach (ProjectSpecification spec in (IEnumerable) projects)
			{
				if (spec.name == requestedProject)
				{
					return spec.logDirectory;
				}
			}

			throw new ApplicationException("Unable to find log configuration for project [ "+ requestedProject + " ]");
		}
	}
}
