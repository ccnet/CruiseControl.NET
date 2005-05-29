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
			string fullQueryString = BuildServerQueryString(serverSpecifier);
			if (queryString != null && queryString != string.Empty)
			{
				fullQueryString += ("&" + queryString);
			}
			return urlBuilder.BuildUrl(action, fullQueryString);
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
			return string.Format("{0}={1}", ServerQueryStringParameter, serverSpecifier.ServerName);
		}

		private string BuildProjectQueryString(IProjectSpecifier projectSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildServerQueryString(projectSpecifier.ServerSpecifier), ProjectQueryStringParameter, projectSpecifier.ProjectName);
		}

		private string BuildBuildQueryString(IBuildSpecifier buildSpecifier)
		{
			return string.Format("{0}&{1}={2}",BuildProjectQueryString(buildSpecifier.ProjectSpecifier), BuildQueryStringParameter, buildSpecifier.BuildName);
		}
	}
}
