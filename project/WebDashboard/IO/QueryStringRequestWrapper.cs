using System;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class QueryStringRequestWrapper : ICruiseRequest
	{
		public static readonly string BuildQueryStringParameter = "build";
		public static readonly string ProjectQueryStringParameter = "project";
		public static readonly string ServerQueryStringParameter = "server";

		private readonly NameValueCollection queryString;

		public QueryStringRequestWrapper(NameValueCollection queryString)
		{
			this.queryString = queryString;
		}

		public IBuildSpecifier GetBuildSpecifier()
		{
			string logfile = queryString[BuildQueryStringParameter];
			if (logfile == null)
			{
				return new NoBuildSpecified();
			}
			else
			{
				return new NamedBuildSpecifier(logfile);
			}
		}

		public string ServerName
		{
			get
			{
				string server = queryString[ServerQueryStringParameter];
				return (server == null) ? "" : server;
			}
		}

		public string ProjectName
		{
			get
			{
				string project = queryString[ProjectQueryStringParameter];
				return (project == null) ? "" : project;
			}
		}

		public string BuildName
		{
			get
			{
				string build = queryString[BuildQueryStringParameter];
				return (build == null) ? "" : build;
			}
		}
	}
}
