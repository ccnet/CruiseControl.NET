using System;
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

		public string BuildUrl(string relativeUrl, string partialQueryString)
		{
			return string.Format("{0}?{1}", BuildUrl(relativeUrl), partialQueryString);
		}

		public string BuildServerUrl(string relativeUrl, string serverName)
		{
			return BuildUrl(relativeUrl, string.Format("{0}={1}", QueryStringRequestWrapper.ServerQueryStringParameter, serverName));
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
