using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorListFactory : IProjectIntegratorListFactory
	{
		public IProjectIntegratorList CreateProjectIntegrators(IProjectList projects)
		{
			ProjectIntegratorList list = new ProjectIntegratorList();
			foreach (IProject project in projects)
			{
				list.Add(new ProjectIntegrator(project));
			}
			return list;
		}
	}
}
