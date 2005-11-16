namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	/// <summary>
	/// Used to provide where the root directory of the application is hosted on the filesystem
	/// </summary>
	public interface IPhysicalApplicationPathProvider
	{
		string PhysicalApplicationPath { get; }
	}
}
