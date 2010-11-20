namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface ICruiseUrlBuilder
	{
		string BuildServerUrl(string action, IServerSpecifier serverSpecifier);
		string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString);
		string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier);
		string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier);
		string Extension { set; get; }
        IUrlBuilder InnerBuilder { get; }
	}
}
