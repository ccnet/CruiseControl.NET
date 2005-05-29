using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultUrlBuilder : IUrlBuilder
	{
		public static readonly string CONTROLLER_RELATIVE_URL = "default.aspx";
		public static readonly string ACTION_PARAMETER_PREFIX = "_action_";

		public string BuildUrl(string action)
		{
			return BuildUrl(action, "");
		}

		public string BuildUrl(string action, string partialQueryString)
		{
			return BuildUrl(action, partialQueryString, CONTROLLER_RELATIVE_URL);
		}

		public string BuildUrl(string action, string partialQueryString, string baseUrl)
		{
			string queryString = string.Format("?{0}{1}=true", ACTION_PARAMETER_PREFIX, action);
			
			if (partialQueryString != null && partialQueryString != string.Empty)
			{
				if (queryString.Length > 1)
				{
					queryString += "&";	
				}
				queryString += partialQueryString;
			}

			return baseUrl + queryString;
		}

		public string BuildFormName(string action)
		{
			return ACTION_PARAMETER_PREFIX + action;
		}
	}
}
