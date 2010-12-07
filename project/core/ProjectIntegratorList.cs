
using System.Collections;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public class ProjectIntegratorList : IProjectIntegratorList
	{
	    private readonly Dictionary<string, IProjectIntegrator> integrators = new Dictionary<string, IProjectIntegrator>();

        /// <summary>
        /// Adds the specified integrator.	
        /// </summary>
        /// <param name="integrator">The integrator.</param>
        /// <remarks></remarks>
		public void Add(IProjectIntegrator integrator)
		{
			Add(integrator.Name, integrator);
		}

        /// <summary>
        /// Adds the specified name.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="integrator">The integrator.</param>
        /// <remarks></remarks>
		public void Add(string name, IProjectIntegrator integrator)
		{
			integrators[name] = integrator;
		}

        /// <summary>
        /// Gets the <see cref="IProjectIntegrator" /> with the specified project name.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public IProjectIntegrator this[string projectName]
		{
			get
			{
                if (!integrators.ContainsKey(projectName)) return null;

			    return integrators[projectName];
			}
		}

        /// <summary>
        /// Gets the count.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
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