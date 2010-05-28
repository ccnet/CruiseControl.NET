
using System.Collections;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorList : IProjectIntegratorList
	{
	    private readonly Dictionary<string, IProjectIntegrator> integrators = new Dictionary<string, IProjectIntegrator>();

		public void Add(IProjectIntegrator integrator)
		{
			Add(integrator.Name, integrator);
		}

		public void Add(string name, IProjectIntegrator integrator)
		{
			integrators[name] = integrator;
		}

		public IProjectIntegrator this[string projectName]
		{
			get { return integrators[projectName]; }
		}

		public int Count
		{
			get { return integrators.Values.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return integrators.Values.GetEnumerator();
		}
	}
}