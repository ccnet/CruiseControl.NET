namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUrlBuilder
	{
		// ToDo - remove this again once Actions implemented for file transfer
		string BuildUrl(string relativeUrl);

		string BuildUrl(IActionSpecifier action);
		string BuildUrl(IActionSpecifier action, string partialQueryString);
		string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier);
		string BuildServerUrl(IActionSpecifier action, IServerSpecifier serverSpecifier, string queryString);
		string BuildProjectUrl(IActionSpecifier action, IProjectSpecifier projectSpecifier);
		string BuildBuildUrl(IActionSpecifier action, IBuildSpecifier buildSpecifier);

		string BuildFormName(IActionSpecifier action, params string[] args);
	}
}
