namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectSpecifier
	{
		string ProjectName { get; }

		IServerSpecifier ServerSpecifier  { get; }
	}
}
