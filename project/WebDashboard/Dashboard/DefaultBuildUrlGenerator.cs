using System;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	// ToDo - create full URL using Query Mapper?
	public class DefaultBuildUrlGenerator : IBuildUrlGenerator
	{
		public string GenerateUrl (string urlBase, string serverName, string projectName, string buildName)
		{
			return string.Format("{0}?{1}={2}&amp;{3}={4}&amp;{5}={6}", urlBase, 
				QueryStringRequestWrapper.ServerQueryStringParameter, serverName,
				QueryStringRequestWrapper.ProjectQueryStringParameter, projectName,
				QueryStringRequestWrapper.BuildQueryStringParameter, buildName);
		}
	}
}
