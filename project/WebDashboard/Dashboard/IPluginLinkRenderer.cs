namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPluginLinkRenderer
	{
		string LinkDescription { get; }
		string LinkActionName { get; }
	}
}
