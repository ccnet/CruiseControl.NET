using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class RequestWrappingCruiseRequest : ICruiseRequest
	{
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
				string server = QueryString[DefaultCruiseUrlBuilder.ServerQueryStringParameter];
				return (server == null) ? "" : server;
			}
		}

		public string ProjectName
		{
			get
			{
				string project = QueryString[DefaultCruiseUrlBuilder.ProjectQueryStringParameter];
				return (project == null) ? "" : project;
			}
		}

		public string BuildName
		{
			get
			{
				string build = QueryString[DefaultCruiseUrlBuilder.BuildQueryStringParameter];
				return (build == null) ? "" : build;
			}
		}

		public IServerSpecifier ServerSpecifier
		{
			get { return new DefaultServerSpecifier(ServerName); }
		}

		public IProjectSpecifier ProjectSpecifier
		{
			get { return new DefaultProjectSpecifier(ServerSpecifier, ProjectName); }
		}

		public IBuildSpecifier BuildSpecifier
		{
			get { return new DefaultBuildSpecifier(ProjectSpecifier, BuildName); }
		}

		public IRequest Request
		{
			get { return request; }
		}
	}
}