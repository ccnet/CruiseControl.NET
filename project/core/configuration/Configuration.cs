namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private ProjectList _projects = new ProjectList();

		public void AddProject(IProject project)
		{
			_projects.Add(project);
		}

		public IProjectList Projects
		{
			get { return _projects; }
		}
	}
}
