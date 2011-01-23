
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IAbsoluteLink
	{
		string Text { get; }
		string Url { get; }

		string LinkClass { set; }
	}
}
