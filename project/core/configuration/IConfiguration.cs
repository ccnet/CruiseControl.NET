namespace ThoughtWorks.CruiseControl.Core
{
	public interface IConfiguration
	{
		IProjectList Projects { get; }
		void AddProject(IProject project);
	}
}
