using System;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		private readonly IPathMapper pathMapper;
		public static readonly string CONTROLLER_RELATIVE_URL = "controller.aspx";

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
					queryString += "&amp;";	
				}
				queryString += partialQueryString;
			}
			return BuildUrl(relativeUrl) + queryString;
		}

		public string BuildServerUrl(string relativeUrl, string serverName)
		{
			return BuildUrl(relativeUrl, BuildServerQueryString(serverName));
		}

		public string BuildServerUrl(IActionSpecifier action, string serverName)
		{
			return BuildUrl(action, BuildServerQueryString(serverName));
		}

		public string BuildProjectUrl(string relativeUrl, string serverName, string projectName)
		{
			return BuildUrl(relativeUrl, BuildProjectQueryString(serverName, projectName));
		}

		public string BuildProjectUrl (IActionSpecifier action, string serverName, string projectName)
		{
			return BuildUrl(action, BuildProjectQueryString(serverName, projectName));
		}

		public string BuildBuildUrl(string relativeUrl, string serverName, string projectName, string buildName)
		{
			return BuildUrl(relativeUrl, BuildBuildQueryString(serverName, projectName, buildName));
		}

		public string BuildBuildUrl(IActionSpecifier action, string serverName, string projectName, string buildName)
		{
			return BuildUrl(action, BuildBuildQueryString(serverName, projectName, buildName));
		}

		private string BuildServerQueryString(string serverName)
		{
			return string.Format("{0}={1}", QueryStringRequestWrapper.ServerQueryStringParameter, serverName);
		}

		private string BuildProjectQueryString(string serverName, string projectName)
		{
			return string.Format("{0}&amp;{1}={2}",BuildServerQueryString(serverName), QueryStringRequestWrapper.ProjectQueryStringParameter, projectName);
		}

		private string BuildBuildQueryString(string serverName, string projectName, string buildName)
		{
			return string.Format("{0}&amp;{1}={2}",BuildProjectQueryString(serverName, projectName), QueryStringRequestWrapper.BuildQueryStringParameter, buildName);
		}
	}
}
