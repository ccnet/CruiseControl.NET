using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

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

		public IServerSpecifier ServerSpecifier
		{
			get
			{
				return new DefaultServerSpecifier(ServerName);
			}
		}

		public IProjectSpecifier ProjectSpecifier
		{
			get
			{
				return new DefaultProjectSpecifier(ServerSpecifier, ProjectName);
			}
		}

		public IBuildSpecifier BuildSpecifier
		{
			get
			{
				return new DefaultBuildSpecifier(ProjectSpecifier, BuildName);
			}
		}

		public IRequest Request
		{
			get { return request; }
		}
	}
}
