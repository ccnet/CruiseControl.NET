namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUrlBuilder
	{
		string BuildUrl(string relativeUrl);
		string BuildUrl(string relativeUrl, string partialQueryString);
		string BuildUrl(IActionSpecifier action);
		string BuildUrl(IActionSpecifier action, string partialQueryString);
		string BuildServerUrl(string relativeUrl, IServerSpecifier serverSpecifier);
		string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier);
		string BuildProjectUrl(string relativeUrl, IProjectSpecifier projectSpecifier);
		string BuildProjectUrl(IActionSpecifier action, IProjectSpecifier projectSpecifier);
		string BuildBuildUrl(string relativeUrl, IBuildSpecifier buildSpecifier);
		string BuildBuildUrl(IActionSpecifier action, IBuildSpecifier buildSpecifier);
	}
}
