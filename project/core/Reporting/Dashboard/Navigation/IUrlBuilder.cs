namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IUrlBuilder
	{
		string BuildUrl(string action);
		string BuildUrl(string action, string partialQueryString);

		string Extension { set; }
	}
}
