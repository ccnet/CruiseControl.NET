using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private ProjectList _projects = new ProjectList();
		private ProjectIntegratorList _integrators = new ProjectIntegratorList();

		public void AddProject(IProject project)
		{
			_projects.Add(project);
			_integrators.Add(new ProjectIntegrator(project));
		}

		public IProjectList Projects
		{
			get { return _projects; }
		}

		public IProjectIntegratorList Integrators
		{
			get { return _integrators; }
		}
	}
}
