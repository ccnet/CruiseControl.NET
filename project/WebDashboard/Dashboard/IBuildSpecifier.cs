namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildSpecifier
	{
		string BuildName { get; }

		IProjectSpecifier ProjectSpecifier  { get; }
	}
}
