using Exortech.NetReflector;
using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public class Configuration : IConfiguration
	{
		private IDictionary _projects = new Hashtable();

		public void AddProject(IProject project)
		{
			_projects[project.Name] = project;
		}

		public IProject GetProject(string name)
		{
			return _projects[name] as IProject;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _projects.Values.GetEnumerator();
		}
	}
}
