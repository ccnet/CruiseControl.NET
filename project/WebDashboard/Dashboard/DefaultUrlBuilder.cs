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

	}
}
