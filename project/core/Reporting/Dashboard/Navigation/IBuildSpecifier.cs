namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IBuildSpecifier
	{
		string BuildName { get; }

		IProjectSpecifier ProjectSpecifier  { get; }
	}
}
