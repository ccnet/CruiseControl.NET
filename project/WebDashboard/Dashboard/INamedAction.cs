using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface INamedAction
	{
		string ActionName { get; }
		ICruiseAction Action { get; }
	}
}
