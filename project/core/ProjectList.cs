using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectList : IProjectList
	{
		private Hashtable _projects = new Hashtable();

		public void Add(IProject project)
		{
			_projects[project.Name] = project;
		}

		public IProject this[string projectName] 
		{ 
			get { return _projects[projectName] as IProject; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _projects.Values.GetEnumerator();
		}

		public void Delete(string name)
		{
			_projects.Remove(name);
		}
	}
}
