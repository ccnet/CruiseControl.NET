using System;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultServerUrlGenerator : IServerUrlGenerator
	{
		public string GenerateUrl (string urlBase, string serverName)
		{
			return string.Format("{0}?{1}={2}", urlBase, QueryStringRequestWrapper.ServerQueryStringParameter, serverName);
		}
	}
}
