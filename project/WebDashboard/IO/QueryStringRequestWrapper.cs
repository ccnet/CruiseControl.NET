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

		public IBuildSpecifier GetBuildSpecifier()
		{
			string logfile = queryString[LogQueryStringParameter];
			if (logfile == null)
			{
				return new NoBuildSpecified();
			}
			else
			{
				return new NamedBuildSpecifier(logfile);
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
