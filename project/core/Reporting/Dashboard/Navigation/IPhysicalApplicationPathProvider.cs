namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public interface IPhysicalApplicationPathProvider
	{
	    string GetFullPathFor(string appRelativePath);
	}
}
