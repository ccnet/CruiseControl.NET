using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private readonly IPathMapper pathMapper;
		public static readonly string CONTROLLER_RELATIVE_URL = "default.aspx";

		public DefaultUrlBuilder(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		public string BuildUrl(string relativeUrl)
		{
			return pathMapper.GetAbsoluteURLForRelativePath(relativeUrl);
		}

		public string BuildUrl(IActionSpecifier action)
		{
			return BuildUrl(action, "");
		}

		public string BuildUrl(string relativeUrl, string partialQueryString)
		{
			return BuildUrl(relativeUrl, new NullActionSpecifier(), partialQueryString);
		}

		public string BuildUrl(IActionSpecifier action, string partialQueryString)
		{
			return BuildUrl(CONTROLLER_RELATIVE_URL, action, partialQueryString);
		}

		private string BuildUrl(string relativeUrl, IActionSpecifier action, string partialQueryString)
		{
			string queryString = "?" + action.ToPartialQueryString();
			
			if (partialQueryString != null && partialQueryString != string.Empty)
			{
				if (queryString.Length > 1)
				{
					queryString += "&";	
				}
				queryString += partialQueryString;
			}
			return BuildUrl(relativeUrl) + queryString;
		}

		public string BuildServerUrl(string relativeUrl, IServerSpecifier serverSpecifier)
		{
			return BuildUrl(relativeUrl, BuildServerQueryString(serverSpecifier));
		}

		public string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier)
		{
			return BuildUrl(action, BuildServerQueryString(serverSpecifier));
		}

		public string BuildProjectUrl(string relativeUrl, IProjectSpecifier projectSpecifier)
		{
			return BuildUrl(relativeUrl, BuildProjectQueryString(projectSpecifier));
		}

		public string BuildProjectUrl(IActionSpecifier action, IProjectSpecifier projectSpecifier)
		{
			return BuildUrl(action, BuildProjectQueryString(projectSpecifier));
		}

		public string BuildBuildUrl(string relativeUrl, IBuildSpecifier buildSpecifier)
		{
			return BuildUrl(relativeUrl, BuildBuildQueryString(buildSpecifier));
		}

		public string BuildBuildUrl(IActionSpecifier action, IBuildSpecifier buildSpecifier)
		{
			return BuildUrl(action, BuildBuildQueryString(buildSpecifier));
		}

		public string BuildFormName(IActionSpecifier action, params string[] args)
		{
			string baseName = CruiseActionFactory.ACTION_PARAMETER_PREFIX + action.ActionName;
			foreach (string arg in args)
			{
				baseName += CruiseActionFactory.ACTION_ARG_SEPARATOR;
				baseName += arg;
			}
			return baseName;
		}

		private string BuildServerQueryString(IServerSpecifier serverSpecifier)
		{
			return string.Format("{0}={1}", RequestWrappingCruiseRequest.ServerQueryStringParameter, serverSpecifier.ServerName);
		}

		private string BuildProjectQueryString(IProjectSpecifier projectSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildServerQueryString(projectSpecifier.ServerSpecifier), RequestWrappingCruiseRequest.ProjectQueryStringParameter, projectSpecifier.ProjectName);
		}

		private string BuildBuildQueryString(IBuildSpecifier buildSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildProjectQueryString(buildSpecifier.ProjectSpecifier), RequestWrappingCruiseRequest.BuildQueryStringParameter, buildSpecifier.BuildName);
		}
	}
}
