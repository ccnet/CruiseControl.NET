namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUrlBuilder
	{
		string BuildUrl(string relativeUrl);
		string BuildUrl(string relativeUrl, string partialQueryString);
		string BuildUrl(string relativeUrl, IActionSpecifier action);
		string BuildUrl(string relativeUrl, IActionSpecifier action, string partialQueryString);
		string BuildServerUrl(string relativeUrl, string serverName);
		string BuildServerUrl(string relativeUrl, IActionSpecifier action, string serverName);
		string BuildProjectUrl(string relativeUrl, string serverName, string projectName);
		string BuildBuildUrl(string relativeUrl, string serverName, string projectName, string buildName);
	}
}
