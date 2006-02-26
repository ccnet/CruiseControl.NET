namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IServerSpecifier
	{
		string ServerName { get; }
		bool AllowForceBuild { get; }
		bool AllowStartStopBuild { get; }
	}
}