
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUrlBuilder
	{
		string BuildUrl(string relativeUrl);
		string BuildUrl(string relativeUrl, string partialQueryString);
	}
}
