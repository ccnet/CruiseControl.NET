using System;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class RequestWrappingCruiseRequest : ICruiseRequest
	{
		public static readonly string BuildQueryStringParameter = "build";
		public static readonly string ProjectQueryStringParameter = "project";
		public static readonly string ServerQueryStringParameter = "server";

		private readonly IRequest request;

		private NameValueCollection QueryString
		{
			get { return request.Params; }
		}

		public RequestWrappingCruiseRequest(IRequest request)
		{
			this.request = request;
		}

		public IBuildSpecifier GetBuildSpecifier()
		{
			string logfile = QueryString[BuildQueryStringParameter];
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
				string server = QueryString[ServerQueryStringParameter];
				return (server == null) ? "" : server;
			}
		}

		public string ProjectName
		{
			get
			{
				string project = QueryString[ProjectQueryStringParameter];
				return (project == null) ? "" : project;
			}
		}

		public string BuildName
		{
			get
			{
				string build = QueryString[BuildQueryStringParameter];
				return (build == null) ? "" : build;
			}
		}

		public IRequest Request
		{
			get { return request; }
		}
	}
}
