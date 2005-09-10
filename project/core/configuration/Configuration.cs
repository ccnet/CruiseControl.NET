
namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private ProjectList projects = new ProjectList();

		public void AddProject(IProject project)
		{
			projects.Add(project);
		}

		public void DeleteProject(string name)
		{
			projects.Delete(name);
		}

		public IProjectList Projects
		{
			get { return projects; }
		}
	}
}