using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class QueryStringRequestWrapper : IRequestWrapper
	{
		public static readonly string LogQueryStringParameter = "log";
		public static readonly string ProjectQueryStringParameter = "project";
		public static readonly string ServerQueryStringParameter = "server";

		private readonly NameValueCollection queryString;

		public QueryStringRequestWrapper(NameValueCollection queryString)
		{
			this.queryString = queryString;
		}

		public ILogSpecifier GetLogSpecifier()
		{
			string logfile = queryString[LogQueryStringParameter];
			if (logfile == null)
			{
				return new NoLogSpecified();
			}
			else
			{
				return new FileNameLogSpecifier(logfile);
			}
		}

		public string GetServerName()
		{
			string server = queryString[ServerQueryStringParameter];
			return (server == null) ? "" : server;
		}

		public string GetProjectName()
		{
			string project = queryString[ProjectQueryStringParameter];
			return (project == null) ? "" : project;
		}

	}
}
