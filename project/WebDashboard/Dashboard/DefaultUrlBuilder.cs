using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private readonly IPathMapper pathMapper;

		public DefaultUrlBuilder(IPathMapper pathMapper)
		{
			this.pathMapper = pathMapper;
		}

		public string BuildUrl(string relativeUrl)
		{
			return pathMapper.GetAbsoluteURLForRelativePath(relativeUrl);
		}

		public string BuildUrl(string relativeUrl, IActionSpecifier action)
		{
			return BuildUrl(relativeUrl, action, "");
		}

		public string BuildUrl(string relativeUrl, string partialQueryString)
		{
			return BuildUrl(relativeUrl, new NullActionSpecifier(), partialQueryString);
		}

		public string BuildUrl(string relativeUrl, IActionSpecifier action, string partialQueryString)
		{
			string queryString = "?" + action.ToPartialQueryString();
			
			if (partialQueryString != null && partialQueryString != string.Empty)
			{
				if (queryString.Length > 1)
				{
					queryString += "&amp;";	
				}
				queryString += partialQueryString;
			}
			return BuildUrl(relativeUrl) + queryString;
		}

		public string BuildServerUrl(string relativeUrl, string serverName)
		{
			return BuildUrl(relativeUrl, string.Format("{0}={1}", QueryStringRequestWrapper.ServerQueryStringParameter, serverName));
		}

		public string BuildServerUrl(string relativeUrl, IActionSpecifier action, string serverName)
		{
			return BuildUrl(relativeUrl, action, string.Format("{0}={1}", QueryStringRequestWrapper.ServerQueryStringParameter, serverName));
		}

		public string BuildProjectUrl(string relativeUrl, string serverName, string projectName)
		{
			return BuildUrl(relativeUrl, string.Format("{0}={1}&amp;{2}={3}", 
				QueryStringRequestWrapper.ServerQueryStringParameter, serverName,
				QueryStringRequestWrapper.ProjectQueryStringParameter, projectName));
		}

		public string BuildBuildUrl(string relativeUrl, string serverName, string projectName, string buildName)
		{
			return BuildUrl(relativeUrl, string.Format("{0}={1}&amp;{2}={3}&amp;{4}={5}", 
				QueryStringRequestWrapper.ServerQueryStringParameter, serverName,
				QueryStringRequestWrapper.ProjectQueryStringParameter, projectName,
				QueryStringRequestWrapper.BuildQueryStringParameter, buildName));
		}
	}
}
