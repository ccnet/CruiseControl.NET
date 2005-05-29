namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IProjectSpecifier
	{
		string ProjectName { get; }

		IServerSpecifier ServerSpecifier  { get; }
	}
}
