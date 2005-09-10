using System.Web;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultCruiseUrlBuilder : ICruiseUrlBuilder
	{
		public static readonly string BuildQueryStringParameter = "build";
		public static readonly string ProjectQueryStringParameter = "project";
		public static readonly string ServerQueryStringParameter = "server";

		private readonly IUrlBuilder urlBuilder;

		public DefaultCruiseUrlBuilder(IUrlBuilder urlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier)
		{
			return BuildServerUrl(action, serverSpecifier, "");
		}

		public string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString)
		{
			return urlBuilder.BuildUrl(action, Combine(BuildServerQueryString(serverSpecifier), queryString));
		}

		public string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier)
		{
			return urlBuilder.BuildUrl(action, BuildProjectQueryString(projectSpecifier));
		}

		public string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier)
		{
			return urlBuilder.BuildUrl(action, BuildBuildQueryString(buildSpecifier));
		}

		public string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier, string fileName)
		{
			return urlBuilder.BuildUrl(action, BuildBuildQueryString(buildSpecifier), fileName);
		}

		private string BuildServerQueryString(IServerSpecifier serverSpecifier)
		{
			return UrlParameter(ServerQueryStringParameter, serverSpecifier.ServerName);
		}

		private string BuildProjectQueryString(IProjectSpecifier projectSpecifier)
		{
			return Combine(BuildServerQueryString(projectSpecifier.ServerSpecifier), UrlParameter(ProjectQueryStringParameter, projectSpecifier.ProjectName));
		}

		private string BuildBuildQueryString(IBuildSpecifier buildSpecifier)
		{
			return Combine(BuildProjectQueryString(buildSpecifier.ProjectSpecifier), UrlParameter(BuildQueryStringParameter, buildSpecifier.BuildName));
		}

		private string UrlParameter(string key, string value)
		{
			return string.Format("{0}={1}", key, HttpUtility.UrlEncode(value));
		}

		private string Combine(string root, string addendum)
		{
			if (StringUtil.IsBlank(addendum)) return root;
			return string.Format("{0}&{1}", root, addendum);
		}
	}
}