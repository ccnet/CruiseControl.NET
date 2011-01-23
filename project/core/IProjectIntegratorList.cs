using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IProjectIntegratorList : IEnumerable
	{
        /// <summary>
        /// Gets the <see cref="IProjectIntegrator" /> with the specified project name.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IProjectIntegrator this[string projectName] { get; }
        /// <summary>
        /// Gets the count.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		int Count { get; }
	}
}
