namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IActionSpecifier
	{
		string ActionName { get; }
		string ToPartialQueryString();
	}
}
