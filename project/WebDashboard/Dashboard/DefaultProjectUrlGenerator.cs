using System;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultProjectUrlGenerator : IProjectUrlGenerator
	{
		public string GenerateUrl (string urlBase, string serverName, string projectName)
		{
			return string.Format("{0}?{1}={2}&amp;{3}={4}", urlBase, 
				QueryStringRequestWrapper.ServerQueryStringParameter, serverName,
				QueryStringRequestWrapper.ProjectQueryStringParameter, projectName);
		}
	}
}
