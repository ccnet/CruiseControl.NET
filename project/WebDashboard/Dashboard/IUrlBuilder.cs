namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUrlBuilder
	{
		string BuildUrl(string relativeUrl);
		string BuildUrl(string relativeUrl, string partialQueryString);
		string BuildUrl(IActionSpecifier action);
		string BuildUrl(IActionSpecifier action, string partialQueryString);
		string BuildServerUrl(string relativeUrl, string serverName);
		string BuildServerUrl(IActionSpecifier action, string serverName);
		string BuildProjectUrl(string relativeUrl, string serverName, string projectName);
		string BuildProjectUrl(IActionSpecifier action, string serverName, string projectName);
		string BuildBuildUrl(string relativeUrl, string serverName, string projectName, string buildName);
		string BuildBuildUrl(IActionSpecifier action, string serverName, string projectName, string buildName);
	}
}
