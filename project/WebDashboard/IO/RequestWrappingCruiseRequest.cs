using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class RequestWrappingCruiseRequest : ICruiseRequest
	{
		private readonly IRequest request;

		public RequestWrappingCruiseRequest(IRequest request)
		{
			this.request = request;
		}

		public string ServerName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ServerRESTSpecifier); }
		}

		public string ProjectName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.ProjectRESTSpecifier); }
		}

		public string BuildName
		{
			get { return FindRESTSpecifiedResource(DefaultCruiseUrlBuilder.BuildRESTSpecifier); }
		}

		private string FindRESTSpecifiedResource(string specifier)
		{
			string[] subFolders = request.SubFolders;

			for (int i = 0; i < subFolders.Length; i += 2)
			{
				if (subFolders[i] == specifier)
				{
					if (i < subFolders.Length)
					{
						return HttpUtility.UrlDecode(subFolders[i + 1]);
					}
					else
					{
						throw new CruiseControlException(
							string.Format("unexpected URL format - found {0} REST Specifier, but no following value", specifier));
					}
				}
			}

			return "";
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