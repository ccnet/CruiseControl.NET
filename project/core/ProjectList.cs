
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectList : IProjectList
	{
		private Hashtable projects = new Hashtable();

		public void Add(IProject project)
		{
			projects[project.Name] = project;
		}

		public IProject this[string projectName] 
		{ 
			get { return projects[projectName] as IProject; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return projects.Values.GetEnumerator();
		}

		public void Delete(string name)
		{
			projects.Remove(name);
		}
	}
}
