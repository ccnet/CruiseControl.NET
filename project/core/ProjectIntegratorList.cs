using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorList : IProjectIntegratorList
	{
		private Hashtable _integrators = new Hashtable();

		public void Add(IProjectIntegrator integrator)
		{
			Add(integrator.Name, integrator);
		}

		public void Add(string name, IProjectIntegrator integrator)
		{
			_integrators[name] = integrator;
		}

		public IProjectIntegrator this[string projectName] 
		{ 
			get { return _integrators[projectName] as IProjectIntegrator; }
		}

		public int Count
		{
			get { return _integrators.Values.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _integrators.Values.GetEnumerator();
		}
	}
}
