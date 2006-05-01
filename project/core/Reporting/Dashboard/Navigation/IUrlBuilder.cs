namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IUrlBuilder
	{
		string BuildUrl(string action);
		string BuildUrl(string action, string queryString);
		string BuildUrl(string action, string queryString, string path);

		string Extension { set; get; }
	}
}
