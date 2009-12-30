#pragma warning disable 1591
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorList : IProjectIntegratorList
	{
		private Hashtable integrators = new Hashtable();

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
			get { return integrators[projectName] as IProjectIntegrator; }
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