namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPlugin
	{
		INamedAction[] NamedActions { get; }
		string LinkDescription { get; }
		string LinkActionName { get; }
	}
}
